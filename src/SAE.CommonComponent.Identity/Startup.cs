using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Identity.Services;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Reflection;

namespace SAE.CommonComponent.Identity
{
    public class Startup : WebPlugin
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
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
            var build = services.AddTransient<ICorsPolicyService,CorsPolicyService>()
                                .AddTransient<IClaimsService, ClaimsService>()
                                .AddIdentityServer()
                                .AddJwtBearerClientAuthentication()
                                .AddDeveloperSigningCredential()
                                .AddClientStore<ClientStoreService>()
                                .AddResourceStore<ResourceStoreService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });


            services.AddSingleton<IdentityOption>();

            services.AddMvc(options =>
                    {
                        options.Filters.Add(new AuthorizeFilter());
                    })
                    .AddResponseResult();

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
            var hostEnvironment= app.ApplicationServices.GetService<IHostEnvironment>();
            if (hostEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
            }
            app.UseServiceFacade();
            app.UseAuthentication();
            app.UseIdentityServer();
        }
    }
}
