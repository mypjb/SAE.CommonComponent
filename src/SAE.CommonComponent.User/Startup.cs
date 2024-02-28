using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.User.Dtos;
using SAE.CommonComponent.User.Converts;
using SAE.CommonComponent.User.Events;
using SAE.CommonLibrary.ObjectMapper;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.ComponentModel;
using System.Reflection;

namespace SAE.CommonComponent.User
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {

        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            TypeDescriptor.AddAttributes(typeof(User.Domains.User), new TypeConverterAttribute(typeof(UserConvert)));

            services.AddTinyMapper()
                    .AddBuilder(builder =>
                    {
                        builder.Bind<UserEvent.ChangePassword, Domains.User>(config =>
                         {
                             config.Bind(@event => @event.Password, user => user.Account.Password);
                         });
                    });

            var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(UserDto).Assembly };

            services.AddMediator(assemblies)
                    // .AddMediatorOrleansProxy()
                    ;

            services.AddMemoryDocument()
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
