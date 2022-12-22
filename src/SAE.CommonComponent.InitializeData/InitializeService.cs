using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.MultiTenant.Commands;
using SAE.CommonComponent.MultiTenant.Dtos;
using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Dtos;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.AspNetCore.Routing;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Extension.Middleware;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.Plugin;
using ClientCommand = SAE.CommonComponent.Application.Commands.ClientCommand;

namespace SAE.CommonComponent.InitializeData
{
    /// <summary>
    /// <see cref="IInitializeService"/>默认实现
    /// </summary>
    /// <inheritdoc/>
    public class InitializeService : IInitializeService
    {
        protected const string SiteMapPath = "siteMap.json";
        protected const string AppSettings = "appsettings.json";
        protected const string SiteMapDevelopmentPath = "siteMap.Development.json";
        protected readonly IMediator _mediator;
        protected readonly ILogging _logging;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IPluginManage _pluginManage;
        private readonly IBitmapEndpointProvider _bitmapEndpointProvider;
        private readonly IPathDescriptorProvider _pathDescriptorProvider;
        private readonly IBitmapAuthorization _bitmapAuthorization;
        private readonly IHostEnvironment _hostEnvironment;

        private readonly SAEOptions _saeOptions;

        public InitializeService(IServiceProvider serviceProvider)
        {
            this._mediator = serviceProvider.GetService<IMediator>();
            var loggingFactory = serviceProvider.GetService<ILoggingFactory>();
            this._logging = loggingFactory.Create(this.GetType().Name);
            this._serviceProvider = serviceProvider;
            this._configuration = this._serviceProvider.GetService<IConfiguration>();
            this._pluginManage = serviceProvider.GetService<IPluginManage>();
            this._bitmapEndpointProvider = serviceProvider.GetService<IBitmapEndpointProvider>();
            this._pathDescriptorProvider = serviceProvider.GetService<IPathDescriptorProvider>();
            this._bitmapAuthorization = serviceProvider.GetService<IBitmapAuthorization>();
            this._hostEnvironment = serviceProvider.GetService<IHostEnvironment>();
        }
        /// <summary>
        /// Find app config
        /// </summary>
        /// <param name="app"></param>
        /// <param name="envId"></param>
        /// <returns></returns>
        protected virtual async Task<IDictionary<string, string>> FindSiteConfigAsync(AppDto appDto, string envId)
        {
            var appPreviewDto = await this._mediator.SendAsync<AppConfigDataPreviewDto>(new AppConfigCommand.Preview
            {
                Id = appDto.Id,
                EnvironmentId = envId
            });
            var appConfig = appPreviewDto.Current.ToJsonString().ToObject<Dictionary<string, object>>();
            IEnumerable<KeyValuePair<string, object>> siteConfigDatas = appConfig.Where(s => s.Key.Equals(SiteConfig.Option, StringComparison.OrdinalIgnoreCase));
            if (siteConfigDatas.Any())
            {
                var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var pairs = siteConfigDatas.First().Value.ToJsonString().ToObject<Dictionary<string, object>>();
                foreach (var kv in pairs)
                {
                    dic[kv.Key] = kv.Value.ToJsonString();
                }
                return dic;
            }

            return new Dictionary<string, string>();
        }
        /// <summary>
        /// Generate app id
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateAppId()
        {
            return Utils.GenerateId();
        }
        protected virtual async Task<IEnumerable<SiteMap>> GetSiteMapsAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var sitePath = string.Empty;

            if (this._hostEnvironment.IsDevelopment())
            {
                sitePath = SiteMapDevelopmentPath;
            }
            else
            {
                sitePath = SiteMapPath;
            }

            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(sitePath));

            if (!resourceName.IsNullOrWhiteSpace())
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using (stream)
                    using (var memory = new MemoryStream())
                    {
                        await stream.CopyToAsync(memory);
                        memory.Position = 0;
                        var json = SAE.CommonLibrary.Constants.Encoding.GetString(memory.ToArray());

                        var siteMaps = json.ToObject<IEnumerable<SiteMap>>();

                        return siteMaps;
                    }
                }
            }

            this._logging.Error($"没有从程序集资源中找到站点地图'{sitePath}'");

            return Enumerable.Empty<SiteMap>();
        }
        protected T GetJTokenValue<T>(JToken token, string name)
        {
            var first = token.FirstOrDefault(s => s.Path.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (first == null)
            {
                var message = $"'{token}' no exist '{name}'";
                this._logging.Error(message);
                throw new ArgumentException(message);
            }
            return token.Value<T>(first.Path);
        }

        protected IEnumerable<T> GetJTokenValues<T>(JToken token, string name)
        {
            var first = token.FirstOrDefault(s => s.Path.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (first == null)
            {
                var message = $"'{token}' no exist '{name}'";
                this._logging.Error(message);
                throw new ArgumentException(message);
            }
            return (token.SelectToken(first.Path) as JArray).Values<T>();
        }

        public virtual async Task InitialAsync(IApplicationBuilder app)
        {
            try
            {
                var dicts = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List());
                if (dicts.Any())
                {
                    this._logging.Warn("系统已初始化过，如需重新初始化，请清理数据后重新启动！");
                    return;
                }

                var totalTime = 0d;

                totalTime += await this.ExecuteCoreAsync($"{nameof(BasicDataAsync)}", this.BasicDataAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(MultiTenantAsync)}", this.MultiTenantAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(ConfigServerAsync)}", this.ConfigServerAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(ApplicationAsync)}", this.ApplicationAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(UserAsync)}", this.UserAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(AuthorizeAsync)}", this.AuthorizeAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(RoutingAsync)}", this.RoutingAsync);

                totalTime += await this.ExecuteCoreAsync($"{nameof(PluginAsync)}", this.PluginAsync);

                this._logging.Info($"total {totalTime}");
            }
            catch (Exception ex)
            {
                this._logging.Error(ex, "Initial fail ");
                app.Run(async (ctx) =>
                {
                    await ctx.Response.WriteAsync($"Initial fail '{ex.Message}'. Please contact the administrator", Encoding.UTF8);
                });
            }
        }

        protected async Task<long> ExecuteCoreAsync(string name, Func<Task> func)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            this._logging.Info($"start initial {name}");
            await func.Invoke();
            this._logging.Info($"end initial {name} elapsed time {stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public virtual async Task BasicDataAsync()
        {

            var scopeId = await this._mediator.SendAsync<string>(new DictCommand.Create
            {
                Name = nameof(DictType.Scope)
            });

            await this._mediator.SendAsync<string>(new DictCommand.Create
            {
                Name = nameof(DictType.Environment)
            });

            var tenantId = await this._mediator.SendAsync<string>(new DictCommand.Create
            {
                Name = nameof(DictType.Tenant)
            });

            var scopeCommand = new DictCommand.Create
            {
                Name = Constants.Scope,
                ParentId = scopeId
            };

            var tenantCommand = new DictCommand.Create
            {
                Name = nameof(DictType.Tenant),
                ParentId = tenantId
            };

            this._logging.Info($"添加区域、租户字典:{scopeCommand.ToJsonString()}\r\n{tenantCommand.ToJsonString()}");

            await this._mediator.SendAsync<string>(scopeCommand);

            await this._mediator.SendAsync<string>(tenantCommand);
        }
        public virtual async Task ApplicationAsync()
        {
            var (cluster, app) = await this.GetAppClusterAsync();

            var environmentDict = await this.GetDictAsync(nameof(DictType.Environment));

            var environments = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                ParentId = environmentDict.ParentId
            });
            var oauthKey = nameof(Constants.Config.OAuth);

            var basicInfoKey = nameof(Constants.Config.BasicInfo);

            var urlKey = nameof(Constants.Config.Url);

            var scopeDict = await this.GetDictAsync(nameof(DictType.Scope));

            var scopes = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                ParentId = scopeDict.Id
            });

            var bitmapEndpoints = await this._bitmapEndpointProvider.ListAsync();

            this._logging.Info($"创建系统资源：{bitmapEndpoints.ToJsonString()}");

            foreach (var bitmapEndpoint in bitmapEndpoints)
            {
                await this._mediator.SendAsync<string>(new AppResourceCommand.Create
                {
                    AppId = app.Id,
                    Method = bitmapEndpoint.Method,
                    Name = $"{bitmapEndpoint.Path}:{bitmapEndpoint.Method}",
                    Path = bitmapEndpoint.Path
                });
            }

            var jsonSeparator = '.';

            var oauthPath = $"{SAE.CommonLibrary.Configuration.Constants.Config.OptionKey.Replace(SAE.CommonLibrary.Configuration.Constants.ConfigSeparator, jsonSeparator)}{jsonSeparator}{oauthKey}";

            foreach (var env in environments)
            {
                var pairs = await FindSiteConfigAsync(app, env.Id);

                if (!pairs.Any() ||
                    !pairs.ContainsKey(oauthKey) ||
                    !pairs.ContainsKey(basicInfoKey) ||
                    !pairs.ContainsKey(urlKey)) continue;

                var oauthJToken = pairs[oauthKey].ToObject<JToken>();

                var basicInfoJToken = pairs[basicInfoKey].ToObject<JToken>();

                var urlJToken = pairs[urlKey].ToObject<JToken>();

                var scopeNames = this.GetJTokenValue<string>(oauthJToken, nameof(Constants.Config.OAuth.Scope)).Split(Constants.Config.OAuth.ScopeSeparator);

                var clientCommand = new ClientCommand.Create
                {
                    AppId = app.Id,
                    Name = this.GetJTokenValue<string>(basicInfoJToken, nameof(Constants.Config.BasicInfo.Name)),
                    Scopes = scopes.Where(s => scopeNames.Contains(s.Name, StringComparer.OrdinalIgnoreCase))
                                   .Select(s => s.Id)
                                   .ToArray(),
                    Endpoint = new ClientEndpointDto
                    {
                        RedirectUris = this.GetJTokenValues<string>(oauthJToken, nameof(ClientEndpointDto.RedirectUris)).ToArray(),
                        PostLogoutRedirectUris = this.GetJTokenValues<string>(oauthJToken, nameof(ClientEndpointDto.PostLogoutRedirectUris)).ToArray(),
                        SignIn = this.GetJTokenValue<string>(urlJToken, nameof(ClientEndpointDto.SignIn))
                    }
                };

                var clientId = await this._mediator.SendAsync<string>(clientCommand);

                // clientCommand.Secret = "************";

                this._logging.Info($"添加默认客户端凭证:{clientCommand.ToJsonString()}");

                await this._mediator.SendAsync(new ClientCommand.ChangeStatus
                {
                    Id = clientId,
                    Status = Status.Enable
                });

                var clientDto = await this._mediator.SendAsync<ClientDto>(new Command.Find<ClientDto>
                {
                    Id = clientId
                });

                var appSettingFileName = $"{Path.GetFileNameWithoutExtension(AppSettings)}.{env.Name}{Path.GetExtension(AppSettings)}";

                var appSettingPath = Path.Combine(AppContext.BaseDirectory, appSettingFileName);

                var appSecret = clientDto.Secret;

                clientDto.Secret = "************";

                this._logging.Info($"默认客户端凭证添加成功:{clientDto.ToJsonString()}");

                if (!File.Exists(appSettingPath))
                {
                    if (env.Name.Equals(Environments.Production, StringComparison.OrdinalIgnoreCase))
                    {
                        appSettingPath = Path.Combine(AppContext.BaseDirectory, AppSettings);
                    }

                    if (!File.Exists(appSettingPath))
                    {
                        this._logging.Warn($"未找到配置文件'{appSettingPath}'，跳过配置变更操作!");
                        break;
                    }
                }

                var appSettingJsonString = await File.ReadAllTextAsync(appSettingPath);

                var appSettingJson = appSettingJsonString.ToObject<JToken>();


                foreach (var jsonPath in oauthPath.Split(jsonSeparator))
                {
                    var array = appSettingJson.Children().ToArray();
                    appSettingJson = (array.First(s =>
                                                  s.Path.Contains(jsonSeparator) ?
                                                  s.Path.EndsWith($"{jsonSeparator}{jsonPath}", StringComparison.OrdinalIgnoreCase) :
                                                  s.Path.Equals(jsonPath, StringComparison.OrdinalIgnoreCase))
                                      as JProperty).Value;
                }

                var properties = appSettingJson.Children<JProperty>();

                var jAppId = properties.First(s => s.Name.Equals(nameof(OAuthOptions.AppId), StringComparison.OrdinalIgnoreCase));
                var jAppSecret = properties.First(s => s.Name.Equals(nameof(OAuthOptions.AppSecret), StringComparison.OrdinalIgnoreCase));
                var jAuthority = properties.First(s => s.Name.Equals(nameof(OAuthOptions.Authority), StringComparison.OrdinalIgnoreCase));
                jAppId.Value = new JValue(clientDto.Id);
                jAppSecret.Value = new JValue(appSecret);
                jAuthority.Value = new JValue(this.GetJTokenValue<string>(oauthJToken,nameof(OAuthOptions.Authority)));
                appSettingJsonString = appSettingJson.Root.ToJsonString();
                await File.WriteAllTextAsync(appSettingPath, appSettingJsonString);
            }
        }

        public virtual async Task AuthorizeAsync()
        {
            var (cluster, app) = await this.GetAppClusterAsync();

            var appResourceDtos = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new AppResourceCommand.List
            {
                AppId = app.Id
            });

            this._logging.Info($"系统资源:{appResourceDtos.ToJsonString()}");

            var roleCommand = new RoleCommand.Create
            {
                AppId = app.Id,
                Description = "超级管理员",
                Name = Constants.Authorize.AdminRoleName
            };

            var roleId = await this._mediator.SendAsync<string>(roleCommand);

            var permissionIds = new string[appResourceDtos.Count()];

            for (int i = 0; i < appResourceDtos.Count(); i++)
            {
                var appResource = appResourceDtos.ElementAt(i);

                var permissionCommand = new PermissionCommand.Create
                {
                    AppId = roleCommand.AppId,
                    Description = appResource.Name,
                    Name = appResource.Name,
                    AppResourceId = appResource.Id
                };

                permissionIds[i] = await this._mediator.SendAsync<string>(permissionCommand);
            }

            var referencePermissionCommand = new RoleCommand.ReferencePermission
            {
                Id = roleId,
                PermissionIds = permissionIds
            };

            await this._mediator.SendAsync(referencePermissionCommand);

            var clientDtos = await this._mediator.SendAsync<IPagedList<ClientDto>>(new ClientCommand.Query
            {
                AppId = app.Id
            });

            foreach (var client in clientDtos)
            {
                await this._mediator.SendAsync(new ClientRoleCommand.ReferenceRole
                {
                    ClientId = client.Id,
                    RoleIds = new[] { roleId }
                });
                var clientCodes = await this._mediator.SendAsync<Dictionary<string, string>>(new ClientRoleCommand.QueryClientAuthorizeCode
                {
                    ClientId = client.Id
                });

                this._logging.Info($"客户端'{client.Name}({client.Id})'认证码:'{clientCodes.ToJsonString()}'");
            }

            var userDtos = await this._mediator.SendAsync<IPagedList<UserDto>>(new UserCommand.Query());

            foreach (var user in userDtos)
            {
                await this._mediator.SendAsync(new UserRoleCommand.ReferenceRole
                {
                    UserId = user.Id,
                    RoleIds = new[] { roleId }
                });

                var userCodes = await this._mediator.SendAsync<Dictionary<string, string>>(new UserRoleCommand.QueryUserAuthorizeCode
                {
                    UserId = user.Id
                });

                this._logging.Info($"用户'{user.Name}({user.Id})'认证码:{userCodes.ToJsonString()}");
            }
        }

        public virtual async Task ConfigServerAsync()
        {
            var configPath = this._configuration.GetValue<string>(SAE.CommonLibrary.Configuration.Constants.Config.RootDirectoryKey);

            var environmentName = this._configuration.GetValue<string>(HostDefaults.EnvironmentKey);

            if (configPath.IsNullOrWhiteSpace() || !Directory.Exists(configPath))
            {
                this._logging.Warn($"未找到对应配置文件'{configPath}'");
                return;
            }

            var (cluster, app) = await this.GetAppClusterAsync();

            var dictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var fileName in Directory.GetFiles(configPath, $"*{Constants.Config.ConfigExtensionName}"))
            {
                this._logging.Info($"加载'{fileName}'配置文件");
                string key = "Production";
                var matchs = Regex.Matches(fileName, $"\\{Constants.Config.Separator}");
                if (matchs.Count > 1)
                {
                    var start = matchs[matchs.Count - 2].Index + 1;
                    var end = matchs[matchs.Count - 1].Index - start;
                    key = fileName.Substring(start, end);
                }
                Dictionary<string, string> pairs;
                if (dictionary.ContainsKey(key))
                {
                    pairs = dictionary[key];
                }
                else
                {
                    pairs = new Dictionary<string, string>();
                    dictionary[key] = pairs;
                }

                foreach (var keyValue in File.ReadAllText(fileName, Encoding.UTF8)
                                             .ToObject<Dictionary<string, object>>())
                {
                    pairs[keyValue.Key] = keyValue.Value.ToJsonString();
                }
            }

            var dicts = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List());

            var environmentParentId = dicts.First(s => s.Name == nameof(DictType.Environment)).Id;

            foreach (var kvs in dictionary)
            {
                var environmentId = await this._mediator.SendAsync<string>(new DictCommand.Create
                {
                    Name = kvs.Key,
                    ParentId = environmentParentId
                });

                this._logging.Info($"创建环境变量'{kvs.Key}'");

                var configIds = new List<string>();

                var publicConfigId = string.Empty;
                foreach (var kv in kvs.Value)
                {
                    var templateDtos = await this._mediator.SendAsync<IEnumerable<TemplateDto>>(new Command.List<TemplateDto>());

                    if (!templateDtos.Any(s => s.Name.Equals(kv.Key)))
                    {
                        this._logging.Info($"创建模板'{kv.Key}'");
                        var templateId = await this._mediator.SendAsync<string>(new TemplateCommand.Create
                        {
                            Format = kv.Value,
                            Name = kv.Key
                        });
                    }
                    this._logging.Info($"创建配置{kv.Key}-{kvs.Key}");
                    var configId = await this._mediator.SendAsync<string>(new ConfigCommand.Create
                    {
                        ClusterId = cluster.Id,
                        Content = kv.Value,
                        Name = kv.Key,
                        EnvironmentId = environmentId
                    });
                    if (kv.Key.Equals(SiteConfig.Option, StringComparison.OrdinalIgnoreCase))
                    {
                        publicConfigId = configId;
                    }
                    configIds.Add(configId);
                }

                this._logging.Info($"引用配置{configIds.Aggregate((a, b) => $"{a},{b}")}");

                await this._mediator.SendAsync(new AppConfigCommand.ReferenceConfig
                {
                    AppId = app.Id,
                    ConfigIds = configIds.ToArray()
                });

                if (!publicConfigId.IsNullOrWhiteSpace())
                {
                    var appConfigs = await this._mediator.SendAsync<IPagedList<AppConfigDto>>(new AppConfigCommand.Query
                    {
                        AppId = app.Id,
                        EnvironmentId = environmentId,
                        PageSize = int.MaxValue
                    });

                    var publicProjectConfig = appConfigs.First(s => s.ConfigId == publicConfigId);

                    await this._mediator.SendAsync(new AppConfigCommand.Change
                    {
                        Alias = publicProjectConfig.Alias,
                        Private = false,
                        Id = publicProjectConfig.Id
                    });
                }

                this._logging.Info($"准备发布系统配置'{app.Name}-{kvs.Key}'");

                await this._mediator.SendAsync(new AppConfigCommand.Publish
                {
                    Id = app.Id,
                    EnvironmentId = environmentId
                });
                this._logging.Info($"发布系统配置'{app.Name}-{kvs.Key}'完成");

                var appConfig = await this._mediator.SendAsync<object>(new AppConfigCommand.Preview
                {
                    Id = app.Id,
                    EnvironmentId = environmentId
                });

                this._logging.Info($"系统配置添加完成。{app.Name}-{kvs.Key} : {appConfig.ToJsonString()}");
            }

        }

        public virtual async Task RoutingAsync()
        {
            IEnumerable<MenuItemDto> menus = await this.GetSiteMapsAsync();
            if (menus?.Any() ?? false)
            {
                this._logging.Info($"加载菜单: {menus.ToJsonString()}");

                await this.AddMenusAsync(menus);

                var dtos = await this._mediator.SendAsync<IEnumerable<MenuItemDto>>(new MenuCommand.Tree());

                this._logging.Info($"菜单加载完成: {dtos.ToJsonString()}");
            }
        }

        protected async Task AddMenusAsync(IEnumerable<MenuItemDto> menus)
        {
            var (cluster, app) = await this.GetAppClusterAsync();

            foreach (var item in menus)
            {
                var command = new MenuCommand.Create
                {
                    Name = item.Name,
                    Path = item.Path,
                    Hidden = item.Hidden,
                    ParentId = item.ParentId,
                    AppId = app.Id
                };
                var parentId = await this._mediator.SendAsync<string>(command);

                item.Id = parentId;

                if (item.Items?.Any() ?? false)
                {
                    item.Items.ForEach(s =>
                {
                    s.ParentId = item.Id;
                });
                    await this.AddMenusAsync(item.Items);
                }
            }
        }

        public virtual async Task UserAsync()
        {
            var command = new AccountCommand.Register
            {
                Name = Constants.User.Name,
                Password = Constants.User.Password,
                ConfirmPassword = Constants.User.Password
            };
            var userId = await this._mediator.SendAsync<string>(command);

            this._logging.Info($"创建默认用户:{command.Name}({userId})");
        }

        public async Task PluginAsync()
        {
            var siteMaps = await this.GetSiteMapsAsync();

            this._logging.Info($"插件管理:{this._pluginManage?.Plugins?.ToJsonString()}");

            var baseUrl = SiteConfig.Get(Constants.Config.Url.Host);

            foreach (var plugin in this._pluginManage.Plugins)
            {
                var siteMap = siteMaps.FirstOrDefault(s => s.Plugin.Equals(plugin.Name, StringComparison.OrdinalIgnoreCase));

                var command = new PluginCommand.Create
                {
                    Name = plugin.Name,
                    Description = plugin.Description,
                    Order = plugin.Order,
                    Status = plugin.Status ? Status.Enable : Status.Disable,
                    Version = plugin.Version
                };
                if (siteMap != null)
                {
                    command.Path = siteMap.Path;
                    if (siteMap.Entry.IsNullOrWhiteSpace())
                    {
                        command.Entry = $"//{plugin.Name}.{baseUrl}".ToLower();
                    }
                    else
                    {
                        command.Entry = siteMap.Entry;
                    }
                }
                await this._mediator.SendAsync<string>(command);
            }

            var plugins = await this._mediator.SendAsync<IEnumerable<PluginDto>>(new Command.List<PluginDto>());

            this._logging.Info($"已加载的插件列表:{plugins?.ToJsonString()}");
        }

        public async Task MultiTenantAsync()
        {
            var dict = await this.GetDictAsync(nameof(DictType.Tenant));

            var clusterCommand = new AppClusterCommand.Create
            {
                Name = Constants.ClusterName,
                Description = "租户管理集群",
                Type = dict.Id
            };

            var clusterId = await this._mediator.SendAsync<string>(clusterCommand);

            this._logging.Info($"创建集群 '{clusterId}'-'{Constants.ClusterName}'");

            var appName = SiteConfig.Get(Constants.Config.BasicInfo.Name);

            var tenantCreateCommand = new TenantCommand.Create
            {
                Name = "默认租户",
                Description = "由系统自动创建的第一个租户",
                Domain = SiteConfig.Get(Constants.Config.Url.Host)
            };

            var tenantId = await this._mediator.SendAsync<string>(tenantCreateCommand);

            var tenantAppCreateCommand = new TenantCommand.App.Create
            {
                Name = appName,
                Description = appName,
                Type = dict.Id,
                Domain = tenantCreateCommand.Type,
                TenantId = tenantId
            };

            await this._mediator.SendAsync<string>(tenantAppCreateCommand);
        }

        private async Task<DictItemDto> GetDictAsync(string type)
        {
            var trees = await this._mediator.SendAsync<IEnumerable<DictItemDto>>(new DictCommand.Tree
            {
                Type = type
            });

            Assert.Build(trees.Any())
                  .True($"字典类型'{type}'不存在！");

            return trees.First();
        }
        private async Task<Tuple<AppClusterDto, AppDto>> GetAppClusterAsync()
        {
            var dict = await this.GetDictAsync(nameof(DictType.Tenant));

            var cluster = await this._mediator.SendAsync<AppClusterDto>(new AppClusterCommand.Find
            {
                Type = dict.Id
            });

            Assert.Build(cluster)
                  .NotNull($"集群'({dict.Name})'不存在!");

            var apps = await this._mediator.SendAsync<IEnumerable<AppDto>>(new AppCommand.List
            {
                ClusterId = cluster.Id
            });

            Assert.Build(apps.Any())
                  .True($"集群'{cluster.Name}'内不存在任何系统!");

            return new Tuple<AppClusterDto, AppDto>(cluster, apps.First());
        }
    }
}
