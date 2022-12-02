using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using AppClusterCommand = SAE.CommonComponent.Application.Commands.AppClusterCommand;
using Assert = Xunit.Assert;
[assembly: InternalsVisibleTo("SAE.CommonComponent.ConfigServer.Test")]

namespace SAE.CommonComponent.Application.Test
{
    public class AppClusterControllerTest : BaseTest
    {
        public const string API = "cluster";

        public AppClusterControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        public AppClusterControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<AppClusterDto> Add()
        {
            var command = new AppClusterCommand.Create
            {
                Name = this.GetRandom(),
                Description = this.GetRandom(),
                Type = this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var app = await this.Get(id);
            this.WriteLine(app);
            Assert.Equal(command.Name, app.Name);
            Assert.Equal(command.Description, app.Description);
            Assert.Equal(command.Type, app.Type);
            return app;
        }

        [Fact]
        public async Task Edit()
        {
            var app = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new AppClusterCommand.Change
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

            var command = new AppClusterCommand.ChangeStatus
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

        private async Task<AppClusterDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}?id={id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var app = await responseMessage.AsAsync<AppClusterDto>();
            this.WriteLine(app);
            return app;
        }
    }
}
