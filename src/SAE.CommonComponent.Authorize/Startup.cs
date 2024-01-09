using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.MessageQueue;
using SAE.CommonLibrary.Plugin.AspNetCore;

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

            var assemblys = new[] { typeof(RuleDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceFacade()
                    .AddMediator(assemblys)
                    //.AddMediatorOrleansProxy()
                    ;

            services.AddBuilder()
                    .AddMemoryDocument()
                    .AddDataPersistenceService(assemblys)
                    .AddMemoryMessageQueue()
                    .AddHandler();

            // services.AddAuthentication()
            //         .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //          {
            //              options.Authority = SiteConfig.Get(Constants.Config.OAuth.Authority);
            //              options.TokenValidationParameters = new TokenValidationParameters
            //              {
            //                  ValidateAudience = false
            //              };
            //              options.RequireHttpsMetadata = false;
            //              options.Events = new JwtBearerEvents();
            //          });


            // services.PostConfigure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            // {
            //     options.ForwardDefaultSelector = (ctx) =>
            //     {
            //         StringValues sv;
            //         if (ctx.Request.Headers.TryGetValue(HttpRequestHeader.Authorization.ToString(), out sv) &&
            //             sv.Any() &&
            //             sv.First().StartsWith(JwtBearerDefaults.AuthenticationScheme))
            //         {
            //             return JwtBearerDefaults.AuthenticationScheme;
            //         }

            //         return null;
            //     };

            // });
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            //app.UseMediatorOrleansSilo();
            app.UseServiceFacade();
            var messageQueue = app.ApplicationServices.GetService<IMessageQueue>();
            // messageQueue.Subscibe<RoleEvent.Create>();
        }
    }
}
