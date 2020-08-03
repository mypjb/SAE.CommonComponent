using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Domains;
using System;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonComponent.ConfigServer.Commands;
using System.Linq;
using SAE.CommonLibrary.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SAE.CommonLibrary.Plugin.AspNetCore;
using Microsoft.Extensions.Logging;

namespace SAE.CommonComponent.ConfigServer
{
    public class Startup:WebPlugin
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder.WithOrigins("http://localhost:8000")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials());
            });

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

            app.UseCors()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            
            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();

            services.AddServiceFacade()
                    .AddMediator();
            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService();
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();
        }
    }
}
