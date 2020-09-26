using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.User.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.InitializeData
{
    public class InitializeService: IInitializeService
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
                Id = Constants.Production.AppId,
                Secret = Constants.Production.Secret,
                Name = Constants.Production.AppName,
                Urls = new[] { Constants.Production.Master }
            };

            await this._mediator.Send<string>(appCommand);

            appCommand.Secret = "************";

            this._logging.Info($"add default app:{appCommand.ToJsonString()}");

            await this._mediator.Send(new AppCommand.ReferenceScope
            {
                Id = Constants.Production.AppId,
                Scopes = new[] { Constants.Scope }
            });

            await this._mediator.Send(new AppCommand.ChangeStatus
            {
                Id = Constants.Production.AppId,
                Status = Status.Enable
            });
        }

        public virtual async Task Authorize()
        {

        }

        public virtual async Task ConfigServer()
        {
        }

        public virtual async Task Initial()
        {
            this._logging.Info($"start initial {nameof(Application)}");
            await this.Application();
            this._logging.Info($"end initial {nameof(Application)}");

            this._logging.Info($"start initial {nameof(Authorize)}");
            await this.Authorize();
            this._logging.Info($"end initial {nameof(Authorize)}");

            this._logging.Info($"start initial {nameof(ConfigServer)}");
            await this.ConfigServer();
            this._logging.Info($"end initial {nameof(ConfigServer)}");

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
                Name = "admin",
                Password = "admin",
                ConfirmPassword = "admin"
            });
        }
    }
}
