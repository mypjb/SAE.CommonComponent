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