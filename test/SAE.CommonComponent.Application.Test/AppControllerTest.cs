using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Application.Test
{
    public class AppControllerTest : BaseTest
    {
        public const string API = "/cluster/app";
        private readonly AppClusterControllerTest _clusterControllerTest;

        public AppControllerTest(ITestOutputHelper output) : base(output)
        {
            this._clusterControllerTest = new AppClusterControllerTest(this._output, this.HttpClient);
        }

        internal AppControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._clusterControllerTest = new AppClusterControllerTest(this._output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Theory]
        [InlineData("")]
        public async Task<AppDto> Add(string clusterId = null)
        {
            if (clusterId.IsNullOrWhiteSpace())
            {
                var clusterDto = await this._clusterControllerTest.Add();
                clusterId = clusterDto.Id;
            }
            var command = new AppCommand.Create
            {
                Id=this.GetRandom(),
                Name = this.GetRandom(),
                ClusterId= clusterId
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var app = await this.Get(id);
            Assert.Equal(command.Id, app.Id);
            Assert.Equal(command.ClusterId, app.ClusterId);
            Assert.Equal(command.Name, app.Name);
            return app;
        }

        [Fact]
        public async Task Edit()
        {
            var app = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new AppCommand.Change
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


        [Fact]
        public async Task ChangeStatus()
        {
            var app = await this.Add();

            var command = new AppCommand.ChangeStatus
            {
                Id = app.Id,
                Status = app.Status == Status.Enable ? Status.Disable : Status.Enable
            };

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/Status")
                                   .AddJsonContent(command);

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var dto = await this.Get(app.Id);

            Assert.Equal(command.Status, dto.Status);
        }



        private async Task<AppDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var app = await responseMessage.AsAsync<AppDto>();
            this.WriteLine(app);
            return app;
        }
    }
}
