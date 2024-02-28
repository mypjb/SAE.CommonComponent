using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAE.CommonComponent.MultiTenant.Dtos;
using SAE.CommonLibrary.Plugin.AspNetCore;

namespace SAE.CommonComponent.MultiTenant
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblies = new[] { typeof(TenantDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceFacade()
                    .AddMediator();

            services.AddMemoryDocument()
                    .AddDataPersistenceService(assemblies)
                    .AddBuilder(assemblies)
                    .AddMemoryMessageQueue()
                    .AddHandler();
        }

        ///<inheritdoc/>
        public override void PluginConfigure(IApplicationBuilder app)
        {
        }
    }
}
