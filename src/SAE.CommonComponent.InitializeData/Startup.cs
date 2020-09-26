using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAE.CommonComponent.InitializeData
{
    public class Startup : WebPlugin
    {
        public override void PluginConfigure(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var initializeService = serviceProvider.GetService<IInitializeService>();
            var mediator = serviceProvider.GetService<IMediator>();
            var command = new AppCommand.Query
            {
                PageIndex = 1,
                PageSize = 1
            };

            var apps = mediator.Send<IPagedList<AppDto>>(command).GetAwaiter().GetResult();

            if (apps.TotalCount == 0)
                initializeService.Initial().GetAwaiter().GetResult();
        }

        public override void PluginConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IInitializeService>(provider =>
            {
                var hostEnvironment = provider.GetService<IHostEnvironment>();

                if (hostEnvironment.IsDevelopment())
                {
                    return new DevelopmentInitializeService(provider);
                }
                else
                {
                    return new InitializeService(provider);
                }
            });
        }
    }
}
