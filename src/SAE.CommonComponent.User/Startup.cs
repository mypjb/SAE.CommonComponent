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
using SAE.CommonComponent.User.Domains;
using SAE.CommonComponent.User.Events;
using SAE.CommonLibrary.ObjectMapper;
using SAE.CommonLibrary.Plugin.AspNetCore;

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
            TinyMapper.Bind<UserEvent.ChangePassword, Domains.User>(config =>
            {
                config.Bind(@event => @event.Password, user => user.Account.Password);
            });

            TinyMapper.Bind<Domains.User, UserDto>(config =>
            {
                config.Bind(user => user.Account.Name, dto => dto.AccountName);
            });


            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();

            services.AddServiceProvider()
                    .AddMediator();
            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddDataPersistenceService(Assembly.GetExecutingAssembly(),
                                               typeof(UserDto).Assembly);
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
            
        }
    }
}
