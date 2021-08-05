using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ClientCommand = SAE.CommonComponent.Application.Commands.ClientCommand;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Application.Test
{
    public class ClientControllerTest : BaseTest
    {
        public const string API = "/client";
        public ClientControllerTest(ITestOutputHelper output) : base(output)
        {
            
        }

        internal ClientControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<ClientDto> Add()
        {
            var command = new ClientCommand.Create
            {
                Name = this.GetRandom(),
                Scopes = new string[] { this.GetRandom(), this.GetRandom() },
                Endpoint = new Dtos.EndpointDto
                {
                    PostLogoutRedirectUris = new[] { this.GetRandom() },
                    RedirectUris = new[] { this.GetRandom() },
                    SignIn = this.GetRandom()
                }
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var app = await this.Get(id);
            this.WriteLine(app);
            Assert.Equal(command.Name, app.Name);
            Assert.Contains(command.Scopes, app.Scopes.Contains);
            Assert.Equal(command.Endpoint.SignIn, app.Endpoint.SignIn);
            Assert.Equal(command.Endpoint.PostLogoutRedirectUris.First(), app.Endpoint.PostLogoutRedirectUris.First());
            Assert.Equal(command.Endpoint.RedirectUris.First(), app.Endpoint.RedirectUris.First());

            return app;
        }

        [Fact]
        public async Task Edit()
        {
            var app = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new ClientCommand.Change
            {
                Id = app.Id,
                Name = this.GetRandom(),
                Scopes = new string[] { this.GetRandom(), this.GetRandom() },
                Endpoint = new Dtos.EndpointDto
                {
                    PostLogoutRedirectUris = new[] { this.GetRandom() },
                    RedirectUris = new[] { this.GetRandom() },
                    SignIn = this.GetRandom()
                }
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newApp = await this.Get(app.Id);
            Assert.Equal(command.Name, newApp.Name);
            Assert.Contains(command.Scopes, newApp.Scopes.Contains);
            Assert.Equal(command.Endpoint.SignIn, newApp.Endpoint.SignIn);
            Assert.Equal(command.Endpoint.PostLogoutRedirectUris.First(), newApp.Endpoint.PostLogoutRedirectUris.First());
            Assert.Equal(command.Endpoint.RedirectUris.First(), newApp.Endpoint.RedirectUris.First());
        }

        [Fact]
        public async Task Refresh()
        {
            var app = await this.Add();

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/{nameof(Refresh)}/{app.Id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var secret = await responseMessage.Content.ReadAsStringAsync();

            var dto = await this.Get(app.Id);

            Assert.NotEqual(app.Secret, dto.Secret);
            this.WriteLine(secret);
        }

        [Fact]
        public async Task ChangeStatus()
        {
            var app = await this.Add();

            var command = new ClientCommand.ChangeStatus
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


        private async Task<ClientDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var app = await responseMessage.AsAsync<ClientDto>();
            this.WriteLine(app);
            return app;
        }
    }
}
