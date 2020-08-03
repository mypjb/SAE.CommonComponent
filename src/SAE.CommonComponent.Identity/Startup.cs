using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Identity.Services;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SAE.CommonComponent.Identity
{
    public class Startup : WebPlugin
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

            app.UseRouting()
               .UseCors();

            this.PluginConfigure(app);

            app.UseEndpoints(endpoints =>
               {
                   endpoints.MapDefaultControllerRoute();
               });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication().AddCookie();

            var build = services.AddIdentityServer()
                                .AddJwtBearerClientAuthentication()
                                .AddDeveloperSigningCredential()
                                .AddClientStore<ClientStoreService>()
                                .AddResourceStore<ResourceStoreService>();

            services.AddSingleton<IdentityOption>();

            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();

            var assemblys = new[] { typeof(AppDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceFacade()
                    .AddMediator(assemblys)
                    //.AddMediatorOrleansClient()
                    ;

            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddSaeMemoryDistributedCache()
                    .AddDataPersistenceService(assemblys);
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();
            app.UseAuthentication();
            app.UseIdentityServer();
        }
    }
}
