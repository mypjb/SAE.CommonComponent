using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using SAE.CommonLibrary.Plugin.AspNetCore;

namespace SAE.CommonComponent.OAuth
{
    public class Startup : WebPlugin
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            this.PluginConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            this.PluginConfigure(app);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            var authenticationBuilder = services.AddAuthentication(options =>
             {
                 options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
             });

            authenticationBuilder.AddOpenIdConnect(options =>
              {
                  options.Authority = Constants.Production.Authority;
                  options.ClientId = Constants.Production.AppId;
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
