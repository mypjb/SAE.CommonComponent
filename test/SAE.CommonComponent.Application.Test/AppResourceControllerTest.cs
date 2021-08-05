using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
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
                AppId= appId,
                Name = this.GetRandom(),
                Method= this.GetRandom(),
                Path= this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var app = await this.Get(id);
            this.WriteLine(app);
            Assert.Equal(command.Name, app.Name);
            return app;
        }

        [Fact]
        public async Task Edit()
        {
            var app = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new AppResourceCommand.Change
            {
                Id = app.Id,
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newApp = await this.Get(app.Id);
            Assert.Equal(command.Name, newApp.Name);
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
