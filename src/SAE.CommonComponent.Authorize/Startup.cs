using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.MessageQueue;
using SAE.CommonLibrary.AspNetCore.Plugin;

namespace SAE.CommonComponent.Authorize
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblies = new[] { typeof(RuleDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMediator()
                    //.AddMediatorOrleansProxy()
                    ;

            services.AddBuilder()
                    .AddMemoryDocument()
                    .AddDataPersistenceService(assemblies)
                    .AddMemoryMessageQueue()
                    .AddHandler();
        }

        ///<inheritdoc/>
        public override void PluginConfigure(IApplicationBuilder app)
        {
            //app.UseMediatorOrleansSilo();
        }
    }
}
