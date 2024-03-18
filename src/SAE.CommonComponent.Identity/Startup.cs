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
using SAE.CommonLibrary.AspNetCore.Plugin;
using System.Reflection;

namespace SAE.CommonComponent.Identity
{
    ///<inheritdoc/>
    public class Startup : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var build = services.AddTransient<ICorsPolicyService, CorsPolicyService>()
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
                    });

            var assemblies = new[] { typeof(ClientDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddMediator()
                    //.AddMediatorOrleansClient()
                    ;

            services.AddMemoryDocument()
                    .AddSAEMemoryDistributedCache()
                    .AddDataPersistenceService(assemblies)
                    .AddMemoryMessageQueue()
                    .AddHandler();
        }
        ///<inheritdoc/>
        public override void PluginConfigure(IApplicationBuilder app)
        {
            var hostEnvironment = app.ApplicationServices.GetService<IHostEnvironment>();
            if (hostEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
            }
            app.UseAuthentication();
            app.UseIdentityServer();
        }
    }
}
