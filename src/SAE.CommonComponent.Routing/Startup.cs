using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary.AspNetCore.Plugin;

namespace SAE.CommonComponent.Routing
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.ApplicationServices.UseServiceFacade();
        }
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblies = new[] { typeof(MenuDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMediator();

            services.AddMemoryDocument()
                    .AddDataPersistenceService(assemblies)
                    .AddMemoryMessageQueue()
                    .AddHandler();
        }
    }
}
