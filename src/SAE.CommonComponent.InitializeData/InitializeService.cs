using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.User.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;
using System.Linq;
using System.Text;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.EventStore.Document;
using Microsoft.Extensions.Hosting;

namespace SAE.CommonComponent.InitializeData
{
    public class InitializeService : IInitializeService
    {
        internal const string ConfigExtensionName = ".json";
        internal const char Separator = '.';
        protected readonly IMediator _mediator;
        protected readonly ILogging _logging;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public InitializeService(IServiceProvider serviceProvider)
        {
            this._mediator = serviceProvider.GetService<IMediator>();
            var loggingFactory = serviceProvider.GetService<ILoggingFactory>();
            this._logging = loggingFactory.Create(this.GetType().Name);
            this._serviceProvider = serviceProvider;
            this._configuration = this._serviceProvider.GetService<IConfiguration>();
        }

        public virtual async Task Application()
        {
            var scopeCommand = new ScopeCommand.Create
            {
                Name = Constants.Scope,
                Display = Constants.Scope
            };

            this._logging.Info($"add default scope:{scopeCommand.ToJsonString()}");

            await this._mediator.Send(scopeCommand);

            var appCommand = new AppCommand.Create
            {
                Id = SiteConfig.Get(Constants.Config.AppId),
                Secret = SiteConfig.Get(Constants.Config.Secret),
                Name = SiteConfig.Get(Constants.Config.AppName),
                Urls = new[] { SiteConfig.Get(Constants.Config.Master) }
            };

            await this._mediator.Send<string>(appCommand);

            appCommand.Secret = "************";

            this._logging.Info($"add default app:{appCommand.ToJsonString()}");

            await this._mediator.Send(new AppCommand.ReferenceScope
            {
                Id = appCommand.Id,
                Scopes = new[] { Constants.Scope }
            });

            await this._mediator.Send(new AppCommand.ChangeStatus
            {
                Id = appCommand.Id,
                Status = Status.Enable
            });
        }

        public virtual async Task Authorize()
        {

        }

        public virtual async Task ConfigServer()
        {
            var configPath = this._configuration.GetValue<string>("SAE:CONFIG:URL");

            var environmentName = this._configuration.GetValue<string>(HostDefaults.EnvironmentKey);

            if (configPath.IsNullOrWhiteSpace() || !Directory.Exists(configPath))
            {
                this._logging.Warn($"Not exist config path '{configPath}'");
                return;
            }

            var slnId = await this._mediator.Send<string>(new SolutionCommand.Create
            {
                Name = Constants.SolutionName
            });

            this._logging.Info($"Create solution '{slnId}'-'{Constants.SolutionName}'");

            var projectName = SiteConfig.Get(Constants.Config.AppName);

            var projectId = await this._mediator.Send<string>(new ProjectCommand.Create
            {
                Name = projectName,
                SolutionId = slnId
            });

            this._logging.Info($"Create project '{projectName}'-'{projectId}'");

            Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var fileName in Directory.GetFiles(configPath, $"*{ConfigExtensionName}"))
            {
                this._logging.Info($"Load '{fileName}' config file");
                string key = "Production";
                if (fileName.Count(s => s.Equals(Separator)) > 1)
                {
                    var start = fileName.IndexOf(Separator) + 1;
                    var length = fileName.LastIndexOf(Separator) - start;
                    key = fileName.Substring(start, length);
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
                var environmentId = await this._mediator.Send<string>(new EnvironmentVariableCommand.Create
                {
                    Name = kvs.Key
                });

                this._logging.Info($"Create EnvironmentVariable '{kvs.Key}'");

                var configIds = new List<string>();

                foreach (var kv in kvs.Value)
                {
                    var templateDtos = await this._mediator.Send<IEnumerable<TemplateDto>>(new Command.List<TemplateDto>());

                    if (!templateDtos.Any(s => s.Name.Equals(kv.Key)))
                    {
                        this._logging.Info($"Create template '{kv.Key}'");
                        var templateId = await this._mediator.Send<string>(new TemplateCommand.Create
                        {
                            Format = kv.Value,
                            Name = kv.Key
                        });
                    }
                    this._logging.Info($"Create config {kv.Key}-{kvs.Key}");
                    var configId = await this._mediator.Send<string>(new ConfigCommand.Create
                    {
                        SolutionId=slnId,
                        Content = kv.Value,
                        Name = kv.Key,
                        EnvironmentId= environmentId
                    });

                    configIds.Add(configId);
                }

                this._logging.Info($"Relevance {configIds.Aggregate((a, b) => $"{a},{b}")}");

                await this._mediator.Send(new ProjectCommand.RelevanceConfig
                {
                    ProjectId = projectId,
                    ConfigIds = configIds
                });

                var appConfigDto = await this._mediator.Send<AppConfigDto>(new ConfigServer.Commands.AppCommand.Config
                {
                    Id = projectId,
                    Env = kvs.Key
                });

                this._logging.Info($"Add project config {projectName}-{kvs.Key} : {appConfigDto.ToJsonString()}");
            }

        }

        public virtual async Task Initial()
        {
            var templates =await this._mediator.Send<IEnumerable<TemplateDto>>(new Command.List<TemplateDto>());
            if (templates.Any()) return;
            this._logging.Info($"start initial {nameof(ConfigServer)}");
            await this.ConfigServer();
            this._logging.Info($"end initial {nameof(ConfigServer)}");

            this._logging.Info($"start initial {nameof(Application)}");
            await this.Application();
            this._logging.Info($"end initial {nameof(Application)}");

            this._logging.Info($"start initial {nameof(Authorize)}");
            await this.Authorize();
            this._logging.Info($"end initial {nameof(Authorize)}");

            this._logging.Info($"start initial {nameof(Routing)}");
            await this.Routing();
            this._logging.Info($"end initial {nameof(Routing)}");

            this._logging.Info($"start initial {nameof(User)}");
            await this.User();
            this._logging.Info($"end initial {nameof(User)}");
        }

        public virtual async Task Routing()
        {
        }

        public virtual async Task User()
        {
            await this._mediator.Send<string>(new UserCommand.Register
            {
                Name = Constants.User.Name,
                Password = Constants.User.Password,
                ConfirmPassword = Constants.User.Password
            });
        }
    }
}
