using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SAE.CommonComponent.BasicData
{

    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblies = new[] { typeof(DictDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMediator();

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
