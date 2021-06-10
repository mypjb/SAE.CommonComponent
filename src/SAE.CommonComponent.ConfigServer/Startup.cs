using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Reflection;

namespace SAE.CommonComponent.ConfigServer
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMediator mediator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            this.PluginConfigure(app);

            app.UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblys = new[] { typeof(ConfigDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMvc()
                    .AddResponseResult();

            services.AddServiceFacade()
                    .AddMediator();
            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(assemblys)
                    .AddBuilder(assemblys);
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();
        }
    }
}
