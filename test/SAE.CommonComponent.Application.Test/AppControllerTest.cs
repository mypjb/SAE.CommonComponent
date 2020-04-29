using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Application.Test
{
    public class AppControllerTest : BaseTest
    {
        public const string API = "app";
        public AppControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<AppDto> Add()
        {
            var command = new AppCommand.Create
            {
                Name = this.GetRandom(),
                Urls = new[] { "localhost" }
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsResult<string>();
            var app = await this.Get(id);
            Assert.Equal(command.Name, app.Name);
            Assert.True(app.Urls.All(command.Urls.Contains));

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
            await responseMessage.AsResult();
            var newApp = await this.Get(app.Id);
            Assert.NotEqual(newApp.Name, app.Name);
        }

        [Fact]
        public async Task Refresh()
        {
            var app = await this.Add();

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/{nameof(Refresh)}/{app.Id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var dto = await this.Get(app.Id);

            Assert.NotEqual(app.Secret, dto.Secret);

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
            return await responseMessage.AsResult<AppDto>();
        }
    }
}
