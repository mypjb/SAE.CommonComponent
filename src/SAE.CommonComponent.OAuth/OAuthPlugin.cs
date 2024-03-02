using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SAE.CommonLibrary;
using SAE.CommonLibrary.AspNetCore.Authorization.ABAC;
using SAE.CommonLibrary.Plugin.AspNetCore;

namespace SAE.CommonComponent.OAuth
{
    ///<inheritdoc/>
    public class OAuthPlugin : WebPlugin
    {
        ///<inheritdoc/>
        public override void PluginConfigureServices(IServiceCollection services)
        {
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });

            services.AddABACAuthorizationWeb();

            authenticationBuilder.AddOpenIdConnect(options =>
            {
                options.Authority = SiteConfig.Get(Constants.Config.OAuth.Authority);
                options.ClientId = SiteConfig.Get(Constants.Config.OAuth.AppId);
                options.ResponseType = "id_token token";
                options.RequireHttpsMetadata = false;
                foreach (var scope in SiteConfig.Get(Constants.Config.OAuth.Scope).Split(Constants.Config.OAuth.ScopeSeparator))
                    options.Scope.Add(scope);
                options.SaveTokens = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.NonceCookie.SameSite = SameSiteMode.Lax;
            });

            services.AddAuthentication()
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                     {
                         options.Authority = SiteConfig.Get(Constants.Config.OAuth.Authority);
                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateAudience = false
                         };
                         options.RequireHttpsMetadata = false;
                         options.Events = new JwtBearerEvents();
                     });


            services.PostConfigure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ForwardDefaultSelector = (ctx) =>
                {
                    StringValues sv;
                    if (ctx.Request.Headers.TryGetValue(HttpRequestHeader.Authorization.ToString(), out sv) &&
                        sv.Any() &&
                        sv.First().StartsWith(JwtBearerDefaults.AuthenticationScheme))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }
                    else
                    {
                        return options.ForwardDefault;
                    }
                };

            });
        }
        ///<inheritdoc/>
        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
