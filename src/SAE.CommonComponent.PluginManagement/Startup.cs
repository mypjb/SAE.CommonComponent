using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Reflection;

namespace SAE.CommonComponent.PluginManagement
{
    public class Startup : WebPlugin
    {
        // This method gets called by the runtime. Use this method to add services to the container.
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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {

            services.AddTinyMapper();


            var assemblies = new[] { Assembly.GetExecutingAssembly(),typeof(Commands.PluginCommand.Create).Assembly };

            services.AddMvc()
                    .AddResponseResult();

            services.AddServiceFacade()
                    .AddMediator(assemblies)
                    // .AddMediatorOrleansProxy()
                    ;

            services.AddMemoryDocument()
                    .AddDataPersistenceService(assemblies)
                    .AddMemoryMessageQueue()
                    .AddHandler();
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();
        }
    }
}
