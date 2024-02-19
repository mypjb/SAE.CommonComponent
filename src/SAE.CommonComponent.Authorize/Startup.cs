using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            //app.UseMediatorOrleansSilo();
            app.UseServiceFacade();
            // var messageQueue = app.ApplicationServices.GetService<IMessageQueue>();
            // messageQueue.Subscibe<RoleEvent.Create>();
        }
    }
}
