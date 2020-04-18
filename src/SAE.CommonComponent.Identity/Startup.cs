using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Abstract.Commands;
using SAE.CommonComponent.Application.Abstract.Dtos;
using SAE.CommonComponent.Identity.Services;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Linq;

namespace SAE.CommonComponent.Identity
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

                Enumerable.Range(0, 10)
                          .Select(s => new AppCreateCommand
                          {
                              Name = s.ToString("000000"),
                              Urls = new[] { $"http://test{s}.com" }
                          }).ForEach(command =>
                          {
                              mediator.Send(command).Wait();
                          });

                mediator.Send(new ScopeCreateCommand() { Name = "config", Display = "config center" }).Wait();
            }

            app.UseRouting()
               .UseCors();
            
            this.PluginConfigure(app);

            app.UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            var build = services.AddIdentityServer()
                                .AddJwtBearerClientAuthentication()
                                .AddDeveloperSigningCredential()
                                .AddClientStore<ClientStoreService>()
                                .AddResourceStore<ResourceStoreService>();

            services.AddSingleton<IdentityOption>();

            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();
            var assembly = typeof(AppDto).Assembly;
            services.AddServiceProvider()
                    .AddMediator(assembly)
                    .AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddSaeMemoryDistributedCache()
                    .AddDataPersistenceService(assembly);
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }
    }
}
