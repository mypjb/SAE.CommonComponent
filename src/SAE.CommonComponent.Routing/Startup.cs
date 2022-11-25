using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary.Plugin.AspNetCore;

namespace SAE.CommonComponent.Routing
{
    public class Startup : WebPlugin
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();
        }
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var assemblys = new[] { typeof(MenuDto).Assembly, Assembly.GetExecutingAssembly() };
            services.AddControllers()
                    .AddResponseResult();

            services.AddServiceFacade()
                    .AddMediator();

            services.AddMemoryDocument()
                    .AddDataPersistenceService(assemblys)
                    .AddMemoryMessageQueue()
                    .AddHandler();
        }
    }
}
