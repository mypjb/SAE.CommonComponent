using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Domains;
using System;

namespace SAE.CommonComponent.ConfigServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddResponseResult()
                    .AddNewtonsoftJson();


            services.AddServiceProvider()
                    .AddMediator()
                    .AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(option =>
                    {
                        option.AddMapper<Template, TemplateDto>();
                        option.AddMapper<Config, ConfigDto>();
                        option.AddMapper<Project, ProjectDto>();
                        option.AddMapper<ProjectConfig, ProjectConfigDto>();
                        option.AddMapper<Solution, SolutionDto>();
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });

        }
    }
}
