using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
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

namespace SAE.CommonComponent.Authorize.Test
{
    public class AppRoleControllerTest : BaseTest
    {
        public const string API = "/App/Role";
        private readonly RoleControllerTest _roleController;

        public AppRoleControllerTest(ITestOutputHelper output) : base(output)
        {
            this._roleController = new RoleControllerTest(output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Theory]
        [InlineData(null)]
        public async Task<string> Reference(string appId = null)
        {
            var command = new ClientRoleCommand.ReferenceRole()
            {
                ClientId = appId.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString("N") : appId
            };

            var roleDtos = new List<RoleDto>();
            await Enumerable.Range(0, 10)
                       .ForEachAsync(async s =>
                       {
                           roleDtos.Add(await this._roleController.ReferencePermission());
                       });

            command.RoleIds = roleDtos.Select(s => s.Id).ToArray();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var roles = await this.Get(command.ClientId);

            Assert.True(roles.All(s => command.RoleIds.Contains(s.Id)));

            return command.ClientId;

        }

        [Fact]
        public async Task DeleteRole()
        {
            var appId = await this.Reference();
            var appRoles = await this.Get(appId);
            var command = new ClientRoleCommand.DeleteRole
            {
                ClientId = appId,
                RoleIds = new[] { appRoles.First().Id }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var roles = await this.Get(command.ClientId);

            Assert.True(!roles.Any(s => command.RoleIds.Contains(s.Id)));
        }



        private async Task<IEnumerable<RoleDto>> Get(string id, bool referenced = true)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/paging?AppId={id}&referenced={referenced}&pagesize={int.MaxValue}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<PagedList<RoleDto>>();
        }
    }
}
