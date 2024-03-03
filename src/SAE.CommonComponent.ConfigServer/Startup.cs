using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Reflection;

namespace SAE.CommonComponent.ConfigServer
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblies = new[] { typeof(ConfigDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMediator();

            services.AddMediatorBehavior()
                    .AddCaching<AppDataCommand.Find,AppDataDto>();

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
