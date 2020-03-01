using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SAE.CommonComponent.Identity
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var build = services.AddIdentityServer()
                                .AddJwtBearerClientAuthentication()
                                .AddDeveloperSigningCredential();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder.WithOrigins("http://localhost:8000")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials());
            });

            services.AddControllers()
                    .AddResponseResult()
                    .AddNewtonsoftJson();

            services.AddServiceProvider()
                    .AddMediator()
                    .AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(option =>
                    {
                        // option.AddMapper<Template, TemplateDto>();
                        // option.AddMapper<Config, ConfigDto>();
                        // option.AddMapper<Project, ProjectDto>();
                        // option.AddMapper<ProjectConfig, ProjectConfigDto>();
                        // option.AddMapper<Solution, SolutionDto>();
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer()
               .UseRouting()
               .UseCors()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }
    }
}
