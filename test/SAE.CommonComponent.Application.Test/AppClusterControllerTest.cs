using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Test;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonLibrary.Abstract.Model;
using AppClusterCommand = SAE.CommonComponent.Application.Commands.AppClusterCommand;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Application.Test
{
    public class AppClusterControllerTest : BaseTest
    {
        public const string API = "cluster";

        public AppClusterControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        internal AppClusterControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
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
                Name = this.GetRandom()
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
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var app = await responseMessage.AsAsync<AppClusterDto>();
            this.WriteLine(app);
            return app;
        }
    }
}
