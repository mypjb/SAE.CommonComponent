using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Abstract.Mediator.Behavior;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using AppResourceCommand = SAE.CommonComponent.Application.Commands.AppResourceCommand;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Application.Test
{
    public class AppResourceControllerTest : BaseTest
    {
        public const string API = "/cluster/app/resource";
        private readonly AppControllerTest _appControllerTest;

        public AppResourceControllerTest(ITestOutputHelper output) : base(output)
        {
            this._appControllerTest = new AppControllerTest(this._output, this.HttpClient);
        }

        internal AppResourceControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._appControllerTest = new AppControllerTest(this._output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>()
                          .ConfigureAppConfiguration(c =>
                          {
                              c.AddInMemoryCollection(new Dictionary<string, string>()
                                                     {
                                                        {$"{RetryPipelineBehaviorOptions.Option}:{nameof(RetryPipelineBehaviorOptions.Num)}","10"}
                                                     });
                          })
                          .ConfigureServices(s =>
                          {
                              s.AddMediatorBehavior()
                               .AddRetry<AppResourceCommand.SetIndex>();
                          });
        }

        [Theory]
        [InlineData("")]
        public async Task<AppResourceDto> Add(string appId = null)
        {
            if (appId.IsNullOrWhiteSpace())
            {
                var appDto = await this._appControllerTest.Add();
                appId = appDto.Id;
            }
            var command = new AppResourceCommand.Create
            {
                AppId = appId,
                Name = this.GetRandom(),
                Method = this.GetRandom(),
                Path = this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var appResource = await this.Get(id);
            // this.WriteLine(appResource);
            Assert.Equal(command.Name, appResource.Name);
            Assert.Equal(command.Method, appResource.Method);
            Assert.Equal(command.Path, appResource.Path);
            Assert.Equal(command.AppId, appResource.AppId);
            return appResource;
        }

        [Fact]
        public async Task Edit()
        {
            var appResource = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new AppResourceCommand.Change
            {
                Id = appResource.Id,
                Name = this.GetRandom(),
                Method = this.GetRandom(),
                Path = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newAppResource = await this.Get(appResource.Id);
            Assert.Equal(command.Name, newAppResource.Name);
            Assert.Equal(command.Method, newAppResource.Method);
            Assert.Equal(command.Path, newAppResource.Path);
        }
        [Fact]
        public async Task BatchAdd()
        {
            var range = new Random().Next(100, 500);
            var appDto = await this._appControllerTest.Add();
            Enumerable.Range(0, range)
                      .AsParallel()
                      .ForAll(s =>
                    //   .ForEach(s =>
                      {
                          this.Add(appDto.Id).GetAwaiter().GetResult();
                      });
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/list?AppId={appDto.Id}");

            var responseMessage = await this.HttpClient.SendAsync(message);
            var resources = await responseMessage.AsAsync<AppResourceDto[]>();
       
            Assert.NotEmpty(resources);
            Assert.Equal(range, resources.Length);
            foreach (var resource in resources)
            {
                Assert.NotEqual(0, resource.Index);
                Assert.Equal(1, resources.Count(s => s.Index == resource.Index));
            }

        }

        private async Task<AppResourceDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var app = await responseMessage.AsAsync<AppResourceDto>();
            this.WriteLine(app);
            return app;
        }
    }
}
