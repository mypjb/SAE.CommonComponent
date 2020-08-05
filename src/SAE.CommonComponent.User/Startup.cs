using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.User.Abstract.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Domains;
using SAE.CommonComponent.User.Events;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Plugin.AspNetCore;
using SAE.CommonLibrary.ObjectMapper;

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
            services.AddTinyMapper()
                    .AddBuilder(builder =>
                    {
                        builder.Bind<UserEvent.ChangePassword, Domains.User>(config =>
                         {
                             config.Bind(@event => @event.Password, user => user.Account.Password);
                         }).Bind<Domains.User, UserDto>(config =>
                         {
                             config.Bind(user => user.Account.Name, dto => dto.AccountName);
                         });
                    });


            var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(UserDto).Assembly };

            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();

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
            var environment = app.ApplicationServices.GetService<IHostEnvironment>();

            if (environment.IsDevelopment())
            {
                var mediator = app.ApplicationServices.GetService<IMediator>();
                mediator.Send<string>(new UserCommand.Register
                {
                    Name = "admin",
                    Password = "admin",
                    ConfirmPassword = "admin"
                }).GetAwaiter().GetResult();
            }
            //app.UseMediatorOrleansSilo();
        }
    }
}
