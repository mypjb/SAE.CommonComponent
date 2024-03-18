using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Converts;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.MessageQueue;
using SAE.CommonLibrary.AspNetCore.Plugin;

namespace SAE.CommonComponent.Application
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            TypeDescriptor.AddAttributes(typeof(Client), new TypeConverterAttribute(typeof(ClientConvert)));

            var assemblies = new[] { typeof(ClientDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMediator()
                    //.AddMediatorOrleansClient()
                    ;
            services.AddMemoryDocument()
                    .AddSAEMemoryDistributedCache()
                    .AddDataPersistenceService(assemblies)
                    .AddMemoryMessageQueue()
                    .AddHandler();

        }
        ///<inheritdoc/>
        public override void PluginConfigure(IApplicationBuilder app)
        {
            var hostEnvironment = app.ApplicationServices.GetService<IHostEnvironment>();
            if (hostEnvironment.IsDevelopment())
            {
                app.Map("/.apps", build =>
                {
                    build.Run(async context =>
                    {
                        var mediator = context.RequestServices.GetService<IMediator>();
                        var paging = await mediator.SendAsync<CommonLibrary.Abstract.Model.IPagedList<ClientDto>>(new ClientCommand.Query());
                        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(paging));
                    });
                });
            }
            var messageQueue = app.ApplicationServices.GetService<IMessageQueue>();
            messageQueue.Subscribe<AppResourceCommand.Create>();
            //app.UseMediatorOrleansSilo();
        }
    }
}
