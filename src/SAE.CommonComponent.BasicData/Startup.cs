using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.AspNetCore.Plugin;
using System.Reflection;

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
