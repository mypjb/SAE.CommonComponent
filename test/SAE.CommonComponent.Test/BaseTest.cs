using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Extension;
using Xunit.Abstractions;

namespace SAE.CommonComponent.Test
{
    // public abstract class BaseTest : MarshalByRefObject
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper _output;
        public IServiceProvider ServiceProvider { get; private set; }
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
                       build.AddJsonFile("appsettings.json");
                       build.AddJsonFile("appsettings.Development.json");
                   })
                   .ConfigureWebHost(webBuilder =>
                   {
                       webBuilder.UseEnvironment(Environments.Development);
                       this.UseStartup(webBuilder.UseTestServer());
                   })
                   .ConfigureDefault(this.Configure)
                   .Start();

            this.ServiceProvider = host.Services;

            var client = new HttpClient(host.GetTestServer().CreateHandler())
            {
                BaseAddress = new Uri("http://localhost:8080")
            };

            client.UseExceptionHandler(async response =>
                        {
                            if (response.StatusCode != System.Net.HttpStatusCode.Found)
                            {
                                var json = await response.Content.ReadAsStringAsync();
                                this.WriteLine(new { Error = json, url = response.RequestMessage.RequestUri });
                                if (json.IsNullOrWhiteSpace())
                                {
                                    throw new SAEException((int)response.StatusCode, json);
                                }
                                var output = json.ToObject<ErrorOutput>();

                                throw new SAEException(output);
                            }
                        });

            this.UseClient(client);
        }

        public BaseTest(ITestOutputHelper output, HttpClient httpClient)
        {
            this._output = output;
            this.HttpClient = httpClient;
            this.ServiceProvider = ServiceFacade.ServiceProvider;
        }
        /// <summary>
        /// 配置<see cref="SAEOptions"/>
        /// </summary>
        /// <param name="options"></param>
        protected virtual void Configure(SAEOptions options)
        {

        }

        protected abstract IWebHostBuilder UseStartup(IWebHostBuilder builder);

        protected void SwitchContext(IServiceProvider serviceProvider)
        {
            new ServiceFacade(serviceProvider);
        }
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
