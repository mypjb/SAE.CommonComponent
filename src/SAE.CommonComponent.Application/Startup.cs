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
using SAE.CommonComponent.Application.Commands;
using System.ComponentModel;
using SAE.CommonComponent.Application.Converts;
using SAE.CommonComponent.Application.Abstract.Domains;

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
            TypeDescriptor.AddAttributes(typeof(Client), new TypeConverterAttribute(typeof(ClientConvert)));

            services.AddMvc()
                    .AddResponseResult();
            var assemblys = new[] { typeof(ClientDto).Assembly, Assembly.GetExecutingAssembly() };

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
            var hostEnvironment = app.ApplicationServices.GetService<IHostEnvironment>();
            if (hostEnvironment.IsDevelopment())
            {
                app.Map("/.apps", build =>
                {
                    build.Run(async context =>
                    {
                        var mediator = context.RequestServices.GetService<IMediator>();
                        var paging = await mediator.SendAsync<CommonLibrary.Abstract.Model.IPagedList<ClientDto>>(new ClientCommand.Query());
                        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(paging));
                    });
                });
            }
            app.UseServiceFacade();
            //app.UseMediatorOrleansSilo();
        }
    }
}
