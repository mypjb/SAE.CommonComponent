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

        public override Task BasicDataAsync()
        {
            return base.BasicDataAsync();
        }
        public override async Task ApplicationAsync()
        {
            await base.ApplicationAsync();
        }

        public override Task AuthorizeAsync()
        {
            return base.AuthorizeAsync();
        }

        protected override string GetProjectId()
        {
            return Guid.Empty.ToString("N");
        }
        public override Task ConfigServerAsync()
        {
            return base.ConfigServerAsync();
        }

        public override Task RoutingAsync()
        {
            return base.RoutingAsync();
        }

        public override Task UserAsync()
        {
            return base.UserAsync();
        }
    }
}
