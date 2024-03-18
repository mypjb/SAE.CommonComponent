using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SAE.CommonComponent.Master
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private readonly IHostEnvironment _env;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            // var _ = typeof(SAE.CommonComponent.Application.Startup);
            // _ = typeof(SAE.CommonComponent.Authorize.Startup);
            // _ = typeof(SAE.CommonComponent.BasicData.Startup);
            // _ = typeof(SAE.CommonComponent.ConfigServer.Startup);
            // _ = typeof(SAE.CommonComponent.Identity.Startup);
            // _ = typeof(SAE.CommonComponent.InitializeData.Startup);
            // _ = typeof(SAE.CommonComponent.MultiTenant.Startup);
            // _ = typeof(SAE.CommonComponent.OAuth.OAuthPlugin);
            // _ = typeof(SAE.CommonComponent.PluginManagement.Startup);
            // _ = typeof(SAE.CommonComponent.Routing.Startup);
            // _ = typeof(SAE.CommonComponent.User.Startup);
            Configuration = configuration;
            this._env = environment;
        }
        /// <summary>
        /// 
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMultiTenant();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddRoutingScanning()
                    .AddServiceFacade();

            services.AddOptions<SiteConfig>(SiteConfig.Option)
                    .Bind();

            if (!this._env.IsDevelopment())
            {
                services.AddMySqlDocument()
                        .AddMongoDB();
            }
            else
            {
                services.AddCors(op =>
                        {
                            op.AddDefaultPolicy(p => p.AllowAnyHeader()
                                                      .AllowAnyMethod()
                                                      .AllowCredentials()
                                                      .SetIsOriginAllowed(t => true)
                                                );
                        });
            }

            services.AddPluginManage(this.Configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            app.ApplicationServices.UseServiceFacade();
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors();
            }

            app.UseMultiTenant()
               .UsePluginManage();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            })
            .UseRoutingScanning();
        }

    }
}
