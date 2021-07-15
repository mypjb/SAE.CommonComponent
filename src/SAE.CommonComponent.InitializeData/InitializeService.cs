using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Dtos;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
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
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;

namespace SAE.CommonComponent.InitializeData
{
    public class InitializeService : IInitializeService
    {
        protected const string SiteMapPath = "siteMap.json";
        protected readonly IMediator _mediator;
        protected readonly ILogging _logging;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IPluginManage _pluginManage;

        public InitializeService(IServiceProvider serviceProvider)
        {
            this._mediator = serviceProvider.GetService<IMediator>();
            var loggingFactory = serviceProvider.GetService<ILoggingFactory>();
            this._logging = loggingFactory.Create(this.GetType().Name);
            this._serviceProvider = serviceProvider;
            this._configuration = this._serviceProvider.GetService<IConfiguration>();
            this._pluginManage = serviceProvider.GetService<IPluginManage>();
        }

        public virtual async Task InitialAsync()
        {
            var templates = await this._mediator.SendAsync<IEnumerable<TemplateDto>>(new Command.List<TemplateDto>());
            if (templates.Any()) return;
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

            this._logging.Info($"start initial {nameof(UserAsync)}");
            await this.UserAsync();
            this._logging.Info($"end initial {nameof(UserAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop();
            totalTime += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            this._logging.Info($"start initial {nameof(PluginAsync)}");
            await this.PluginAsync();
            this._logging.Info($"end initial {nameof(PluginAsync)} elapsed time {stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop();
            totalTime += stopwatch.ElapsedMilliseconds;;

            this._logging.Info($"total {totalTime}");
        }

        public virtual async Task BasicDataAsync()
        {
            var scopeCommand = new DictCommand.Create
            {
                Name = Constants.Scope,
                Type = (int)DictType.Scope
            };

            this._logging.Info($"add default scope:{scopeCommand.ToJsonString()}");

            await this._mediator.SendAsync(scopeCommand);
        }
        public virtual async Task ApplicationAsync()
        {
            var solutions = await this._mediator.SendAsync<IPagedList<SolutionDto>>(new SolutionCommand.Query());
            if (!solutions.Any())
            {
                return;
            }
            var solution = solutions.First();

            var projects = await this._mediator.SendAsync<IPagedList<ProjectDto>>(new ProjectCommand.Query
            {
                SolutionId = solution.Id
            });

            if (!projects.Any()) { return; }

            var environments = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Environment
            });

            foreach (var project in projects)
            {
                foreach (var env in environments)
                {
                    var pairs = await FindConfigAsync(project, env.Name);

                    if (!pairs.Any()) continue;

                    var appCommand = new AppCommand.Create
                    {
                        Id = pairs[Constants.Config.AppId],
                        Secret = pairs[Constants.Config.Secret],
                        Name = pairs[Constants.Config.AppName],
                        Endpoint = new EndpointDto
                        {
                            RedirectUris = pairs[nameof(EndpointDto.RedirectUris)].ToObject<string[]>(),
                            PostLogoutRedirectUris = pairs[nameof(EndpointDto.PostLogoutRedirectUris)].ToObject<string[]>(),
                            SignIn = pairs[nameof(EndpointDto.SignIn)]
                        }
                    };

                    await this._mediator.SendAsync<string>(appCommand);

                    appCommand.Secret = "************";

                    this._logging.Info($"add default app:{appCommand.ToJsonString()}");

                    await this._mediator.SendAsync(new AppCommand.ReferenceScope
                    {
                        Id = appCommand.Id,
                        Scopes = new[] { Constants.Scope }
                    });

                    await this._mediator.SendAsync(new AppCommand.ChangeStatus
                    {
                        Id = appCommand.Id,
                        Status = Status.Enable
                    });

                    var app = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto>
                    {
                        Id = appCommand.Id
                    });
                    app.Secret = appCommand.Secret;
                    this._logging.Info($"output default app:{app.ToJsonString()}");
                }
            }
        }

        private async Task<IDictionary<string, string>> FindConfigAsync(ProjectDto project, string env)
        {
            var appConfig = await this._mediator.SendAsync<AppConfigDto>(new ConfigServer.Commands.AppCommand.Config
            {
                Id = project.Id,
                Env = env
            });
            IEnumerable<KeyValuePair<string, object>> siteConfigDatas = appConfig.Data
                                            .Where(s => s.Key.Equals(SiteConfig.Option, StringComparison.OrdinalIgnoreCase));
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

        public virtual async Task AuthorizeAsync()
        {

        }
        protected virtual string GetProjectId()
        {
            return Utils.GenerateId();
        }
        public virtual async Task ConfigServerAsync()
        {
            var configPath = this._configuration.GetValue<string>(SAE.CommonLibrary.Configuration.Constants.ConfigRootDirectoryKey);

            var environmentName = this._configuration.GetValue<string>(HostDefaults.EnvironmentKey);

            if (configPath.IsNullOrWhiteSpace() || !Directory.Exists(configPath))
            {
                this._logging.Warn($"Not exist config path '{configPath}'");
                return;
            }

            var slnId = await this._mediator.SendAsync<string>(new SolutionCommand.Create
            {
                Name = Constants.SolutionName
            });

            this._logging.Info($"Create solution '{slnId}'-'{Constants.SolutionName}'");

            var projectName = SiteConfig.Get(Constants.Config.AppName);

            var projectId = await this._mediator.SendAsync<string>(new ProjectCommand.Create
            {
                Name = projectName,
                SolutionId = slnId,
                Id=this.GetProjectId()
            });

            this._logging.Info($"Create project '{projectName}'-'{projectId}'");

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
                        SolutionId = slnId,
                        Content = kv.Value,
                        Name = kv.Key,
                        EnvironmentId = environmentId
                    });

                    configIds.Add(configId);
                }

                this._logging.Info($"Relevance {configIds.Aggregate((a, b) => $"{a},{b}")}");

                await this._mediator.SendAsync(new ProjectCommand.RelevanceConfig
                {
                    ProjectId = projectId,
                    ConfigIds = configIds.ToArray()
                });

                this._logging.Info($"Publish {projectName}-{kvs.Key} config");
                await this._mediator.SendAsync(new ProjectCommand.Publish
                {
                    Id = projectId,
                    EnvironmentId = environmentId
                });

                var appConfigDto = await this._mediator.SendAsync<AppConfigDto>(new ConfigServer.Commands.AppCommand.Config
                {
                    Id = projectId,
                    Env = kvs.Key
                });

                this._logging.Info($"Add project config {projectName}-{kvs.Key} : {appConfigDto.ToJsonString()}");
            }

        }

        public virtual async Task RoutingAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(SiteMapPath));

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
                        var json = SAE.CommonLibrary.Constant.Encoding.GetString(memory.ToArray());

                        var menus = json.ToObject<IEnumerable<MenuItemDto>>();

                        this._logging.Info($"local menus : {menus.ToJsonString()}");

                        await this.AddMenusAsync(menus);

                        var dtos = await this._mediator.SendAsync<IEnumerable<MenuItemDto>>(new MenuCommand.Tree());

                        this._logging.Info($"remote menu : {dtos}");
                    }
                    return;
                }
            }
            this._logging.Warn($"The assembly resource listing not find '{SiteMapPath}'");
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
            this._logging.Info($"Plugin manage:{this._pluginManage?.Plugins?.ToJsonString()}");
            foreach (var plugin in this._pluginManage.Plugins)
            {
                var command = new PluginCommand.Create
                {
                    Name = plugin.Name,
                    Description = plugin.Description,
                    Order = plugin.Order,
                    Status = plugin.Status ? Status.Enable : Status.Disable,
                    Version = plugin.Version
                };
                await this._mediator.SendAsync<string>(command);
            }
            var plugins = await this._mediator.SendAsync<IEnumerable<PluginDto>>(new Command.List<PluginDto>());
            this._logging.Info($"Plugin list:{plugins?.ToJsonString()}");
        }

        
    }
}
