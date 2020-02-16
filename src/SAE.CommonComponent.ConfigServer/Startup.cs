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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMediator mediator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                Enumerable.Range(0, 100)
                          .ForEach(s =>
                          {
                              mediator.Send<string>(new SolutionCreateCommand
                              {
                                  Name = $"测试解决方案{s}"
                              });
                          });
            }

            app.UseRouting()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });

        }
    }
}
