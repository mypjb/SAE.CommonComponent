using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.AspNetCore.Routing;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace SAE.CommonComponent.Authorize
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
                endpoints.MapDefaultControllerRoute();
            });
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddResponseResult();

            var assemblys = new[] { typeof(UserRoleDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceFacade()
                    .AddMediator(assemblys)
                    //.AddMediatorOrleansProxy()
                    ;

            services.AddBuilder()
                    .AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(assemblys);

            services.AddBitmapAuthorization()
                    .AddLocalBitmapEndpointProvider(provider =>
                    {
                        var mediator = provider.GetService<IMediator>();

                        return mediator.Send<Command.List<BitmapEndpoint>, IEnumerable<BitmapEndpoint>>(new Command.List<BitmapEndpoint>())
                                       .GetAwaiter()
                                       .GetResult();
                    });

            services.AddAuthentication()
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                     {
                         options.Authority = SiteConfig.Get(Constants.Config.Authority);
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

                    return null;
                };

            });
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            //app.UseMediatorOrleansSilo();
            app.UseServiceFacade();
        }
    }
}
