using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace SAE.CommonComponent.Application
{
    public class Startup : WebPlugin
    {
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

            app.UseAuthorization();

            this.PluginConfigure(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public override void PluginConfigureServices(IServiceCollection services)
        {

            services.AddMvc()
                    .AddResponseResult()
                    .AddNewtonsoftJson();
            var assemblys = new[] { typeof(AppDto).Assembly, Assembly.GetExecutingAssembly() };

            services.AddServiceFacade()
                    .AddMediator(assemblys)
                    //.AddMediatorOrleansClient()
                    ;
            services.AddMemoryDocument()
                    .AddMemoryMessageQueue()
                    .AddSaeMemoryDistributedCache()
                    .AddDataPersistenceService(assemblys);
        }

        public override void PluginConfigure(IApplicationBuilder app)
        {
            app.UseServiceFacade();

            var environment = app.ApplicationServices.GetService<IHostEnvironment>();

            if (environment.IsDevelopment())
            {
                var mediator = app.ApplicationServices.GetService<IMediator>();
                var scopeCommand = new Commands.ScopeCommand.Create
                {
                    Name = Constants.Scope,
                    Display = Constants.Scope
                };
                mediator.Send(scopeCommand).GetAwaiter().GetResult();

                var appCommand = new Commands.AppCommand.Create
                {
                    Id = Constants.DefalutAppId,
                    Secret = Constants.DefalutSecret,
                    Name = Constants.DefalutAppName,
                    Urls = new[] { Constants.DefaultMaster }
                };

                mediator.Send<string>(appCommand).GetAwaiter().GetResult();

                mediator.Send(new Commands.AppCommand.ReferenceScope
                {
                    Id = Constants.DefalutAppId,
                    Scopes = new[] { Constants.Scope }
                }).GetAwaiter().GetResult();

                mediator.Send(new Commands.AppCommand.ChangeStatus
                {
                    Id = Constants.DefalutAppId,
                    Status = Status.Enable
                }).GetAwaiter().GetResult();

                app.Map("/.apps", build =>
                {
                    build.Run(async context =>
                    {
                        var paging = await mediator.Send<CommonLibrary.Abstract.Model.IPagedList<AppDto>>(new Commands.AppCommand.Query());
                        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(paging));
                    });
                });
            }
            //app.UseMediatorOrleansSilo();
        }
    }
}
