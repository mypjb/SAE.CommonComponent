using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Test;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Authorize.Test
{
    public class PermissionControllerTest : BaseTest
    {
        public const string API = "permission";
        public PermissionControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        internal PermissionControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Theory]
        [InlineData("", "")]
        public async Task<PermissionDto> Add(string appId = null, string appResourceId = null)
        {
            var command = new PermissionCommand.Create
            {
                Name = $"test_{this.GetRandom()}",
                Description = "add permission",
                AppId = appId.IsNullOrWhiteSpace() ? this.GetRandom() : appId,
                AppResourceId = appResourceId.IsNullOrWhiteSpace() ? this.GetRandom() : appResourceId
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var id = await httpResponse.AsAsync<string>();
            var permission = await this.Get(id);

            Assert.Equal(permission.Name, command.Name);
            Assert.Equal(permission.Description, command.Description);
            this.WriteLine(permission);
            return permission;
        }
        [Fact]
        public async Task Edit()
        {
            var dto = await this.Add();
            var command = new PermissionCommand.Change
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = "edit permission"
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var permission = await this.Get(dto.Id);

            Assert.Equal(permission.Name, command.Name);
            Assert.Equal(permission.Description, command.Description);

            this.WriteLine(permission);
        }

        [Fact]
        public async Task Delete()
        {
            var dto = await this.Add();

            var command = new Command.BatchDelete<Permission>
            {
                Ids = new[] { dto.Id }
            };
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var exception = await Assert.ThrowsAnyAsync<SAEException>(() => this.Get(dto.Id));

            Assert.Equal(exception.Code, (int)StatusCodes.ResourcesNotExist);

        }

        [Fact]
        public async Task Location()
        {
            var commands = Enumerable.Range(0, 1000)
                      .Select(s => new PermissionCommand.Create
                      {
                          Name = $"localtion_{s}",
                          Description = $"location {s}"
                      });

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Location)}");

            request.AddJsonContent(commands);

            var httpResponse = await this.HttpClient.SendAsync(request);

            var endpoints = await httpResponse.AsAsync<IEnumerable<BitmapEndpoint>>();

            foreach (var command in commands)
            {

            }
            this.WriteLine(endpoints);
        }


        private async Task<PermissionDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<PermissionDto>();
        }
    }
}
