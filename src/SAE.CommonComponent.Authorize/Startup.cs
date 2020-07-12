using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.AspNetCore.Routing;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Plugin.AspNetCore;

namespace SAE.CommonComponent.Authorize
{
    public class Startup:WebPlugin
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            this.PluginConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            this.PluginConfigure(app);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .AddResponseResult();

            var assemblys = new[] { typeof(UserRoleDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceProvider()
                    .AddMediator(assemblys)
                    .AddMediatorOrleansProxy();

            services.AddBuilder()
                    .AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(assemblys);

            services.AddBitmapAuthorization()
                    .AddLocalBitmapEndpointProvider(provider =>
                    {
                        var mediator = provider.GetService<IMediator>();
                        
                        return mediator.Send<Command.List<BitmapEndpoint>,IEnumerable<BitmapEndpoint>>(new Command.List<BitmapEndpoint>())
                                       .GetAwaiter()
                                       .GetResult();
                    });

        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseMediatorOrleansSilo();
            //app.UseRoutingScanning()
            //   .UseBitmapAuthorization();
        }
    }
}
