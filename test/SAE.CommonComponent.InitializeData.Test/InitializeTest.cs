using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Plugin;
using SAE.CommonLibrary.Plugin.AspNetCore;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
namespace SAE.CommonComponent.InitializeData.Test
{
    public class InitializeTest : BaseTest
    {
        public InitializeTest(ITestOutputHelper output) : base(output)
        {
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            var _ = typeof(SAE.CommonComponent.Application.Startup);
            _ = typeof(SAE.CommonComponent.Authorize.Startup);
            _ = typeof(SAE.CommonComponent.BasicData.Startup);
            _ = typeof(SAE.CommonComponent.ConfigServer.Startup);
            _ = typeof(SAE.CommonComponent.Identity.Startup);
            _ = typeof(SAE.CommonComponent.InitializeData.Startup);
            _ = typeof(SAE.CommonComponent.MultiTenant.Startup);
            _ = typeof(SAE.CommonComponent.OAuth.OAuthPlugin);
            _ = typeof(SAE.CommonComponent.PluginManagement.Startup);
            _ = typeof(SAE.CommonComponent.Routing.Startup);
            _ = typeof(SAE.CommonComponent.User.Startup);
            return builder.UseStartup<Master.Startup>();
        }

        [Fact]
        public async Task InitialTest()
        {
            // var configuration = this.ServiceProvider.GetService<IConfiguration>();
            // var path = configuration.GetSection("plugin:path").Value;
            //var directoryInfo = new DirectoryInfo("/home/pjb/Documents/github/personals/SAE.CommonComponent/test/SAE.CommonComponent.InitializeData.Test/bin/Debug/net7.0/../../../../../plugins");
            var pluginManage = this.ServiceProvider.GetService<IPluginManage>();
            this.WriteLine(new { pluginManage.Plugins });
            Assert.NotEmpty(pluginManage.Plugins);
            var rep = await this.HttpClient.GetAsync("/menu/tree");
            var json = await rep.Content.ReadAsStringAsync();
            this.WriteLine(json);
            rep.EnsureSuccessStatusCode();
        }
    }
}