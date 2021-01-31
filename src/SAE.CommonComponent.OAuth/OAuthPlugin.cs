using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonLibrary.Plugin.AspNetCore;

namespace SAE.CommonComponent.OAuth
{
    public class OAuthPlugin : WebPlugin
    {
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var authenticationBuilder = services.AddAuthentication(options =>
             {
                 options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
             });


            authenticationBuilder.AddOpenIdConnect(options =>
            {
                options.Authority = SiteConfig.Get(Constants.Config.Authority);
                options.ClientId = SiteConfig.Get(Constants.Config.AppId);
                options.ResponseType = "id_token token";
                options.RequireHttpsMetadata = false;
                options.Scope.Add(Constants.Scope);
                options.SaveTokens = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                options.NonceCookie.SameSite = SameSiteMode.Unspecified;
            });
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
