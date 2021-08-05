using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Application.Test
{

    public class ClientAppResourceControllerTest : BaseTest
    {
        public const string API = "/client/appresource";
        private readonly AppResourceControllerTest _appResourceControllerTest;

        public ClientAppResourceControllerTest(ITestOutputHelper output) : base(output)
        {
            this._appResourceControllerTest = new AppResourceControllerTest(output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<string> Reference()
        {
            var command = new ClientAppResourceCommand.ReferenceAppResource()
            {
                ClientId = this.GetRandom(),
                AppResourceIds=new string[]
                {
                   (await this._appResourceControllerTest.Add()).Id,
                   (await this._appResourceControllerTest.Add()).Id,
                   (await this._appResourceControllerTest.Add()).Id,
                   (await this._appResourceControllerTest.Add()).Id
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var appResources = await this.Get(command.ClientId);

            Assert.True(appResources.All(s => command.AppResourceIds.Contains(s.Id)));

            return command.ClientId;

        }

        [Fact]
        public async Task DeleteRole()
        {
            var clientId = await this.Reference();
            var appResources = await this.Get(clientId);
            var command = new ClientAppResourceCommand.DeleteAppResource
            {
                ClientId = clientId,
                AppResourceIds = new[] { appResources.First().Id }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var newAppResources = await this.Get(command.ClientId);

            Assert.True(!newAppResources.Any(s => command.AppResourceIds.Contains(s.Id)));
        }



        private async Task<IEnumerable<AppResourceDto>> Get(string id, bool referenced = true)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/paging?clientId={id}&referenced={referenced}&pagesize={int.MaxValue}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<PagedList<AppResourceDto>>();
        }
    }
}
