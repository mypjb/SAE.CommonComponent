using SAE.CommonComponent.Application.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public class DevelopmentInitializeService : InitializeService
    {
        public DevelopmentInitializeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override async Task Application()
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
                Id = Constants.Development.AppId,
                Secret = Constants.Development.Secret,
                Name = Constants.Development.AppName,
                Urls = new[] { Constants.Development.Master }
            };

            this._logging.Info($"add default app:{appCommand.ToJsonString()}");
            await this._mediator.Send<string>(appCommand);

            await this._mediator.Send(new AppCommand.ReferenceScope
            {
                Id = Constants.Development.AppId,
                Scopes = new[] { Constants.Scope }
            });

            await this._mediator.Send(new AppCommand.ChangeStatus
            {
                Id = Constants.Development.AppId,
                Status = Status.Enable
            });
        }

        public override Task Authorize()
        {
            return base.Authorize();
        }

        public override Task ConfigServer()
        {
            return base.ConfigServer();
        }

        public override Task Routing()
        {
            return base.Routing();
        }

        public override Task User()
        {
            return base.User();
        }
    }
}
