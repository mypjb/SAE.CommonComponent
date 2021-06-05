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
            //var scopeCommand = new ScopeCommand.Create
            //{
            //    Name = Constants.Scope,
            //    Display = Constants.Scope
            //};

            //this._logging.Info($"add default scope:{scopeCommand.ToJsonString()}");

            //await this._mediator.SendAsync(scopeCommand);

            //var appCommand = new AppCommand.Create
            //{
            //    Id = SiteConfig.Get(Constants.Config.AppId),
            //    Secret = SiteConfig.Get(Constants.Config.Secret),
            //    Name = SiteConfig.Get(Constants.Config.AppName),
            //    Urls = new[] { SiteConfig.Get(Constants.Config.Master) }
            //};

            //this._logging.Info($"add default app:{appCommand.ToJsonString()}");
            //await this._mediator.SendAsync<string>(appCommand);

            //await this._mediator.SendAsync(new AppCommand.ReferenceScope
            //{
            //    Id = appCommand.Id,
            //    Scopes = new[] { Constants.Scope }
            //});

            //await this._mediator.SendAsync(new AppCommand.ChangeStatus
            //{
            //    Id = appCommand.Id,
            //    Status = Status.Enable
            //});

            await base.Application();
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
