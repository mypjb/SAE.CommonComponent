using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using SAE.CommonLibrary.Plugin;
using SAE.CommonLibrary.Plugin.AspNetCore;

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

            services.AddMultiTenant();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddRoutingScanning()
                    .AddServiceFacade()
                    .AddSAECors();

            services.AddOptions<SiteConfig>(SiteConfig.Option)
                    .Bind();

            if (!this._env.IsDevelopment())
            {
                services.AddMySqlDocument()
                        .AddMongoDB();
            }
            services.AddPluginManage(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            app.UseServiceFacade();
            app.UseRouting()
               .UseMultiTenant()
               .UseSAECors()
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
