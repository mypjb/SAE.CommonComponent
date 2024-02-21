using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.Plugin.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public class Startup : WebPlugin
    {
        private const string KeyPath = "keys";
        public override void PluginConfigure(IApplicationBuilder app)
        {

            //if (File.Exists(KeyPath))
            //{
            //    return;
            //}

            var provider = app.ApplicationServices;

            var hostEnvironment = provider.GetService<IHostEnvironment>();

            IInitializeService initializeService;

            if (hostEnvironment.IsDevelopment())
            {
                initializeService = new DevelopmentInitializeService(provider);
            }
            else
            {
                initializeService = new InitializeService(provider);
            }
            initializeService.InitialAsync(app).GetAwaiter().GetResult();
            //var key = Guid.NewGuid().ToString("N");

            //File.AppendAllText(KeyPath, key);
        }


        public override void PluginConfigureServices(IServiceCollection services)
        {

        }
    }
}
