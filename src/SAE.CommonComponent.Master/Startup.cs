using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAE.CommonLibrary.Database;
using SAE.CommonLibrary.Plugin;

namespace SAE.CommonComponent.Master
{
    public class Startup
    {
        private readonly IHostEnvironment _env;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            this._env = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddRoutingScanning()
                    .AddServiceFacade()
                    .AddNlogLogger();

            services.AddDefaultConfiguration();
            if (!this._env.IsDevelopment())
            {
                services.AddMySqlDocument()
                        .AddMongoDB();
            }

            services.AddPluginManage(this._env.IsDevelopment() ? "../../../../../plugin" : string.Empty);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPluginManage pluginManage)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseServiceFacade();
            app.UseRouting()
               .UseCors()
               .UseAuthentication()
               .UseAuthorization()
               .UsePluginManage()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               })
               .UseRoutingScanning();
        }
    }
}
