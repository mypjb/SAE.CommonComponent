using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using System.Collections.Generic;
using System.Net.Http;
using Xunit.Abstractions;

namespace SAE.CommonComponent.Test
{
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper _output;
        protected HttpClient HttpClient
        {
            get; private set;
        }
        public BaseTest(ITestOutputHelper output)
        {
            _output = output;
            var host = new HostBuilder()
                   .ConfigureAppConfiguration(build =>
                   {
                       var dict = new Dictionary<string, string>
                        {
                           {"SAE:CONFIG:URL", "http://localhost:8080/app/config?id=0dbbcfdf123f44baad50ac830106c87b&env=Development"},
                           {HostDefaults.ApplicationKey,"SAE.CommonComponent.Master" }
                        };
                       build.AddInMemoryCollection(dict);
                   })
                   .ConfigureWebHost(webBuilder =>
                   {
                       webBuilder.UseEnvironment(Environments.Development);
                       this.UseStartup(webBuilder.UseTestServer());
                   }).ConfigureDefault()
                   .Start();

            this.UseClient(host.GetTestClient()
                               .UseDefaultExceptionHandler());
        }

        public BaseTest(ITestOutputHelper output, HttpClient httpClient)
        {
            this._output = output;
            this.HttpClient = httpClient;
        }

        protected abstract IWebHostBuilder UseStartup(IWebHostBuilder builder);
        public virtual void UseClient(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }
        /// <summary>
        /// 打印输出
        /// </summary>
        /// <param name="object"></param>
        protected void WriteLine(object @object)
        {
            this._output.WriteLine(@object.ToJsonString());
        }
        /// <summary>
        /// 获得随机值
        /// </summary>
        /// <returns></returns>
        protected string GetRandom()
        {
            return Utils.GenerateId().ToMd5(true).Replace("-", string.Empty);
        }

    }
}
