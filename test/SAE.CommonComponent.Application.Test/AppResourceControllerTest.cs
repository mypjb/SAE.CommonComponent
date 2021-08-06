using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using System;
using System.Net.Http;
using System.Threading.Tasks;
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
            return builder.UseStartup<Startup>();
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
                Path = this.GetRandom(),
                Index = Math.Abs(this.GetRandom().GetHashCode() % 100)
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var appResource = await this.Get(id);
            this.WriteLine(appResource);
            Assert.Equal(command.Name, appResource.Name);
            Assert.Equal(command.Method, appResource.Method);
            Assert.Equal(command.Index, appResource.Index);
            Assert.Equal(command.Path, appResource.Path);
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
                Method=this.GetRandom(),
                Path=this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newAppResource = await this.Get(appResource.Id);
            Assert.Equal(command.Name, newAppResource.Name);
            Assert.Equal(command.Method, newAppResource.Method);
            Assert.Equal(command.Path, newAppResource.Path);
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
