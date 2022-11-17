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
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClientCommand = SAE.CommonComponent.Application.Commands.ClientCommand;

namespace SAE.CommonComponent.InitializeData
{
    public class InitializeService : IInitializeService
    {
        protected const string SiteMapPath = "siteMap.json";
        protected const string SiteMapDevelopmentPath = "siteMap.Development.json";
        protected readonly IMediator _mediator;
        protected readonly ILogging _logging;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IPluginManage _pluginManage;
        private readonly IBitmapEndpointProvider _bitmapEndpointProvider;
        private readonly IPathDescriptorProvider _pathDescriptorProvider;
        private readonly IBitmapAuthorization _bitmapAuthorization;
        private readonly SystemOptions _systemOptions;
        private readonly IHostEnvironment _hostEnvironment;

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
            this._systemOptions = _configuration.GetSection(SystemOptions.Option)
                                                .Get<SystemOptions>();
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
                        var json = Constant.Encoding.GetString(memory.ToArray());

                        var siteMaps = json.ToObject<IEnumerable<SiteMap>>();

                        return siteMaps;
                    }
                }
            }

            this._logging.Error($"The assembly resource listing not find '{sitePath}'");

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
                var dicts = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
                {
                    Type = (int)DictType.Scope
                });
                if (dicts.Any()) return;

                var totalTime = 0d;
                var stopwatch = new Stopwatch();

                stopwatch.Start();
                this._logging.Info($"start initial {nameof(BasicDataAsync)}");
                await this.BasicDataAsync();
                this._logging.Info($"end initial {nameof(BasicDataAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                this._logging.Info($"start initial {nameof(ConfigServerAsync)}");
                await this.ConfigServerAsync();
                this._logging.Info($"end initial {nameof(ConfigServerAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                this._logging.Info($"start initial {nameof(ApplicationAsync)}");
                await this.ApplicationAsync();
                this._logging.Info($"end initial {nameof(ApplicationAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                this._logging.Info($"start initial {nameof(UserAsync)}");
                await this.UserAsync();
                this._logging.Info($"end initial {nameof(UserAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                this._logging.Info($"start initial {nameof(AuthorizeAsync)}");
                await this.AuthorizeAsync();
                this._logging.Info($"end initial {nameof(AuthorizeAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                this._logging.Info($"start initial {nameof(RoutingAsync)}");
                await this.RoutingAsync();
                this._logging.Info($"end initial {nameof(RoutingAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                this._logging.Info($"start initial {nameof(PluginAsync)}");
                await this.PluginAsync();
                this._logging.Info($"end initial {nameof(PluginAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();
                totalTime += stopwatch.ElapsedMilliseconds; ;

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

        public virtual async Task BasicDataAsync()
        {
            var scopeCommand = new DictCommand.Create
            {
                Name = Constants.Scope,
                Type = (int)DictType.Scope
            };

            this._logging.Info($"Add default scope:{scopeCommand.ToJsonString()}");

            await this._mediator.SendAsync<string>(scopeCommand);
        }
        public virtual async Task ApplicationAsync()
        {
            var appClusters = await this._mediator.SendAsync<IPagedList<AppClusterDto>>(new AppClusterCommand.Query());

            if (!appClusters.Any())
            {
                return;
            }

            var appQueryCommand = new AppCommand.Query
            {
                ClusterId = appClusters.First().Id,
                PageSize = int.MaxValue
            };

            var appDtos = await this._mediator.SendAsync<IPagedList<AppDto>>(appQueryCommand);

            var environments = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Environment
            });
            var oauthKey = nameof(Constants.Config.OAuth);

            var basicInfoKey = nameof(Constants.Config.BasicInfo);

            var urlKey = nameof(Constants.Config.Url);

            var scopes = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Scope
            });

            var bitmapEndpoints = await this._bitmapEndpointProvider.FindsAsync(this._pathDescriptorProvider.GetDescriptors());

            var appFirst = appDtos.First();

            foreach (var bitmapEndpoint in bitmapEndpoints)
            {
                await this._mediator.SendAsync<string>(new AppResourceCommand.Create
                {
                    AppId = appFirst.Id,
                    Method = bitmapEndpoint.Method,
                    Name = $"{bitmapEndpoint.Path}:{bitmapEndpoint.Method}",
                    Path = bitmapEndpoint.Path,
                    Index = bitmapEndpoint.Index
                });
            }

            foreach (var appDto in appDtos)
            {
                foreach (var env in environments)
                {
                    var pairs = await FindSiteConfigAsync(appDto, env.Id);

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
                        Id = this.GetJTokenValue<string>(oauthJToken, nameof(Constants.Config.OAuth.AppId)),
                        Secret = this.GetJTokenValue<string>(oauthJToken, nameof(Constants.Config.OAuth.AppId)),
                        AppId = appDto.Id,
                        Name = this.GetJTokenValue<string>(basicInfoJToken, nameof(Constants.Config.BasicInfo.Name)),
                        Scopes = scopes.Where(s => scopeNames.Contains(s.Name, StringComparer.OrdinalIgnoreCase))
                                       .Select(s => s.Id)
                                       .ToArray(),
                        Endpoint = new EndpointDto
                        {
                            RedirectUris = this.GetJTokenValues<string>(oauthJToken, nameof(EndpointDto.RedirectUris)).ToArray(),
                            PostLogoutRedirectUris = this.GetJTokenValues<string>(oauthJToken, nameof(EndpointDto.PostLogoutRedirectUris)).ToArray(),
                            SignIn = this.GetJTokenValue<string>(urlJToken, nameof(EndpointDto.SignIn))
                        }
                    };

                    await this._mediator.SendAsync<string>(clientCommand);

                    clientCommand.Secret = "************";

                    this._logging.Info($"Add default client:{clientCommand.ToJsonString()}");

                    await this._mediator.SendAsync(new ClientCommand.ChangeStatus
                    {
                        Id = clientCommand.Id,
                        Status = Status.Enable
                    });

                    var clientDto = await this._mediator.SendAsync<ClientDto>(new Command.Find<ClientDto>
                    {
                        Id = clientCommand.Id
                    });

                    clientDto.Secret = clientCommand.Secret;

                    this._logging.Info($"output default app:{clientDto.ToJsonString()}");

                    //var clientAppResourceCommand = new ClientAppResourceCommand.ReferenceAppResource
                    //{
                    //    AppResourceIds = appResourceDtos.Select(s => s.Id).ToArray(),
                    //    ClientId = clientDto.Id
                    //};

                    //await this._mediator.SendAsync(clientAppResourceCommand);

                    //var appResources = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new ClientAppResourceCommand.List
                    //{
                    //    ClientId = clientDto.Id
                    //});

                    //var authorizeCode = this._bitmapAuthorization.GeneratePermissionCode(appResources.Select(s => s.Index));
                    //this._logging.Info($"app '{clientDto.Id}' authorize code:{authorizeCode}");
                }
            }
        }

        public virtual async Task AuthorizeAsync()
        {
            var appResourceDtos = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new AppResourceCommand.List
            {
                AppId = this._systemOptions.Id
            });

            this._logging.Info($"app resource:{appResourceDtos.ToJsonString()}");

            var roleCommand = new RoleCommand.Create
            {
                AppId = this._systemOptions.Id,
                Description = "Default admin role",
                Name = Constants.Authorize.AdminRoleName
            };

            var roleId = await this._mediator.SendAsync<string>(roleCommand);

            var permissionIds = new string[appResourceDtos.Count()];

            for (int i = 0; i < appResourceDtos.Count(); i++)
            {
                var appResource = appResourceDtos.ElementAt(i);
                var path = string.Format(Constants.Authorize.PermissionFormat, appResource.Method, appResource.Path);
                var permissionCommand = new PermissionCommand.Create
                {
                    AppId = roleCommand.AppId,
                    Description = appResource.Name,
                    Name = path,
                    Path = path
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
                AppId = this._systemOptions.Id,
                PageSize = int.MaxValue
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

                this._logging.Info($"Client '{client.Id}' Codes:{clientCodes.ToJsonString()}");
            }

            var userDtos = await this._mediator.SendAsync<IPagedList<UserDto>>(new UserCommand.Query
            {
                PageSize = int.MaxValue
            });

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

                this._logging.Info($"User '{user.Id}' Codes:{userCodes.ToJsonString()}");
            }
        }

        public virtual async Task ConfigServerAsync()
        {
            var configPath = this._configuration.GetValue<string>(SAE.CommonLibrary.Configuration.Constant.Config.RootDirectoryKey);

            var environmentName = this._configuration.GetValue<string>(HostDefaults.EnvironmentKey);

            if (configPath.IsNullOrWhiteSpace() || !Directory.Exists(configPath))
            {
                this._logging.Warn($"Not exist config path '{configPath}'");
                return;
            }

            var clusterCommand = new AppClusterCommand.Create
            {
                Name = Constants.ClusterName
            };

            var clusterId = await this._mediator.SendAsync<string>(clusterCommand);

            this._logging.Info($"Create solution '{clusterId}'-'{Constants.ClusterName}'");

            var appName = SiteConfig.Get(Constants.Config.BasicInfo.Name);

            var appCommand = new AppCommand.Create
            {
                Id = _systemOptions.Id,
                Name = appName,
                ClusterId = clusterId
            };

            var appId = await this._mediator.SendAsync<string>(appCommand);

            this._logging.Info($"Create app '{appName}'-'{appId}'");

            Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var fileName in Directory.GetFiles(configPath, $"*{Constants.Config.ConfigExtensionName}"))
            {
                this._logging.Info($"Load '{fileName}' config file");
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

            foreach (var kvs in dictionary)
            {
                var environmentId = await this._mediator.SendAsync<string>(new DictCommand.Create
                {
                    Name = kvs.Key,
                    Type = (int)DictType.Environment
                });

                this._logging.Info($"Create EnvironmentVariable '{kvs.Key}'");

                var configIds = new List<string>();

                var publicConfigId = string.Empty;
                foreach (var kv in kvs.Value)
                {
                    var templateDtos = await this._mediator.SendAsync<IEnumerable<TemplateDto>>(new Command.List<TemplateDto>());

                    if (!templateDtos.Any(s => s.Name.Equals(kv.Key)))
                    {
                        this._logging.Info($"Create template '{kv.Key}'");
                        var templateId = await this._mediator.SendAsync<string>(new TemplateCommand.Create
                        {
                            Format = kv.Value,
                            Name = kv.Key
                        });
                    }
                    this._logging.Info($"Create config {kv.Key}-{kvs.Key}");
                    var configId = await this._mediator.SendAsync<string>(new ConfigCommand.Create
                    {
                        ClusterId = clusterId,
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

                this._logging.Info($"Reference {configIds.Aggregate((a, b) => $"{a},{b}")}");

                await this._mediator.SendAsync(new AppConfigCommand.ReferenceConfig
                {
                    AppId = appId,
                    ConfigIds = configIds.ToArray()
                });

                if (!publicConfigId.IsNullOrWhiteSpace())
                {
                    var appConfigs = await this._mediator.SendAsync<IPagedList<AppConfigDto>>(new AppConfigCommand.Query
                    {
                        AppId = appId,
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

                this._logging.Info($"Publish {appName}-{kvs.Key} config");

                await this._mediator.SendAsync(new AppConfigCommand.Publish
                {
                    Id = appId,
                    EnvironmentId = environmentId
                });
                this._logging.Info($"Publish {appName}-{kvs.Key} config");

                var appConfig = await this._mediator.SendAsync<object>(new AppConfigCommand.Preview
                {
                    Id = appId,
                    EnvironmentId = environmentId
                });

                this._logging.Info($"Add app config {appName}-{kvs.Key} : {appConfig.ToJsonString()}");
            }

        }

        public virtual async Task RoutingAsync()
        {
            IEnumerable<MenuItemDto> menus = await this.GetSiteMapsAsync();
            if (menus?.Any() ?? false)
            {
                this._logging.Info($"local menus : {menus.ToJsonString()}");

                await this.AddMenusAsync(menus);

                var dtos = await this._mediator.SendAsync<IEnumerable<MenuItemDto>>(new MenuCommand.Tree());

                this._logging.Info($"remote menu : {dtos}");
            }
        }

        protected async Task AddMenusAsync(IEnumerable<MenuItemDto> menus)
        {
            foreach (var item in menus)
            {
                var command = new MenuCommand.Create
                {
                    Name = item.Name,
                    Path = item.Path,
                    Hidden = item.Hidden,
                    ParentId = item.ParentId
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
            await this._mediator.SendAsync<string>(new AccountCommand.Register
            {
                Name = Constants.User.Name,
                Password = Constants.User.Password,
                ConfirmPassword = Constants.User.Password
            });
        }

        public async Task PluginAsync()
        {
            var siteMaps = await this.GetSiteMapsAsync();
            this._logging.Info($"Plugin manage:{this._pluginManage?.Plugins?.ToJsonString()}");
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
            this._logging.Info($"Plugin list:{plugins?.ToJsonString()}");
        }

    }
}
