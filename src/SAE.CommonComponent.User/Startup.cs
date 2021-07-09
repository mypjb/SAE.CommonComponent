using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.User.Dtos;
using SAE.CommonComponent.User.Converts;
using SAE.CommonComponent.User.Events;
using SAE.CommonLibrary.ObjectMapper;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.ComponentModel;
using System.Reflection;

namespace SAE.CommonComponent.User
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

        public override void PluginConfigureServices(IServiceCollection services)
        {
            TypeDescriptor.AddAttributes(typeof(User.Domains.User), new TypeConverterAttribute(typeof(UserConvert)));

            services.AddTinyMapper()
                    .AddBuilder(builder =>
                    {
                        builder.Bind<UserEvent.ChangePassword, Domains.User>(config =>
                         {
                             config.Bind(@event => @event.Password, user => user.Account.Password);
                         });
                    });


            var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(UserDto).Assembly };

            services.AddMvc()
                    .AddResponseResult();

            services.AddServiceFacade()
                    .AddMediator(assemblies)
                    // .AddMediatorOrleansProxy()
                    ;

            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(assemblies);
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

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();
            //app.UseMediatorOrleansSilo();
        }
    }
}
