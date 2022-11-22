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
        private readonly AppControllerTest _appControllerTest;

        public ClientControllerTest(ITestOutputHelper output) : base(output)
        {
            this._appControllerTest = new AppControllerTest(this._output, this.HttpClient);
        }

        internal ClientControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._appControllerTest = new AppControllerTest(this._output, this.HttpClient);
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
                AppId = (await this._appControllerTest.Add()).Id,
                Name = this.GetRandom(),
                Scopes = new string[] { this.GetRandom(), this.GetRandom() },
                Endpoint = new Dtos.ClientEndpointDto
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
            var client = await this.Get(id);

            Assert.Equal(command.AppId, client.AppId);
            Assert.Equal(command.Name, client.Name);
            Assert.Contains(command.Scopes, client.Scopes.Contains);
            Assert.Equal(command.Endpoint.SignIn, client.Endpoint.SignIn);
            Assert.Equal(command.Endpoint.PostLogoutRedirectUris.First(), client.Endpoint.PostLogoutRedirectUris.First());
            Assert.Equal(command.Endpoint.RedirectUris.First(), client.Endpoint.RedirectUris.First());

            return client;
        }

        [Fact]
        public async Task Edit()
        {
            var client = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new ClientCommand.Change
            {
                Id = client.Id,
                Name = this.GetRandom(),
                Scopes = new string[] { this.GetRandom(), this.GetRandom() },
                Endpoint = new Dtos.ClientEndpointDto
                {
                    PostLogoutRedirectUris = new[] { this.GetRandom() },
                    RedirectUris = new[] { this.GetRandom() },
                    SignIn = this.GetRandom()
                }
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newClient = await this.Get(client.Id);
            Assert.Equal(command.Name, newClient.Name);
            Assert.Contains(command.Scopes, newClient.Scopes.Contains);
            Assert.Equal(command.Endpoint.SignIn, newClient.Endpoint.SignIn);
            Assert.Equal(command.Endpoint.PostLogoutRedirectUris.First(), newClient.Endpoint.PostLogoutRedirectUris.First());
            Assert.Equal(command.Endpoint.RedirectUris.First(), newClient.Endpoint.RedirectUris.First());
        }

        [Fact]
        public async Task Refresh()
        {
            var client = await this.Add();

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/{nameof(Refresh)}/{client.Id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var secret = await responseMessage.Content.ReadAsStringAsync();

            var dto = await this.Get(client.Id);

            Assert.NotEqual(client.Secret, dto.Secret);
            this.WriteLine(secret);
        }

        [Fact]
        public async Task ChangeStatus()
        {
            var client = await this.Add();

            var command = new ClientCommand.ChangeStatus
            {
                Id = client.Id,
                Status = client.Status == Status.Enable ? Status.Disable : Status.Enable
            };

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/Status")
                                   .AddJsonContent(command);

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var dto = await this.Get(client.Id);

            Assert.Equal(command.Status, dto.Status);
        }


        private async Task<ClientDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var client = await responseMessage.AsAsync<ClientDto>();
            this.WriteLine(client);
            return client;
        }
    }
}
