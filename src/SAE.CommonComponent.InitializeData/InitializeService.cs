using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.User.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Threading.Tasks;
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;

namespace SAE.CommonComponent.InitializeData
{
    public class InitializeService : IInitializeService
    {
        protected readonly IMediator _mediator;
        protected readonly ILogging _logging;

        public InitializeService(IServiceProvider serviceProvider)
        {
            this._mediator = serviceProvider.GetService<IMediator>();
            var loggingFactory = serviceProvider.GetService<ILoggingFactory>();
            this._logging = loggingFactory.Create(this.GetType().Name);
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
            var slnId =await this._mediator.Send<string>(new SolutionCommand.Create
            {
                Name = Constants.SolutionName
            });

            var projectId = await this._mediator.Send<string>(new ProjectCommand.Create
            {
                Name = SiteConfig.Get(Constants.Config.AppName),
                SolutionId = slnId
            });

            await this._mediator.Send<string>(new TemplateCommand.Create
            {
                Name = ""
            });

        }

        public virtual async Task Initial()
        {
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
