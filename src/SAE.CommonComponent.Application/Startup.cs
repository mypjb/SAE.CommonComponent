using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Reflection;

namespace SAE.CommonComponent.Application
{
    public class Startup : WebPlugin
    {
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

            app.UseAuthorization();

            this.PluginConfigure(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public override void PluginConfigureServices(IServiceCollection services)
        {

            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();
            var assemblys =new[] { typeof(AppDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceProvider()
                    .AddMediator(assemblys)
                    .AddMediatorOrleansClient();
            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddSaeMemoryDistributedCache()
                    .AddDataPersistenceService(assemblys);
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseMediatorOrleansSilo();
        }
    }
}
