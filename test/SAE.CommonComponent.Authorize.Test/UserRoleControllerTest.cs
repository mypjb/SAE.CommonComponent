using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
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
    public class UserRoleControllerTest : BaseTest
    {
        public const string API = "UserRole";
        private readonly PermissionControllerTest _permissionController;
        private readonly RoleControllerTest _roleController;

        public UserRoleControllerTest(ITestOutputHelper output) : base(output)
        {
            this._permissionController = new PermissionControllerTest(output, this.HttpClient);
            this._roleController = new RoleControllerTest(output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Theory]
        [InlineData(null)]
        public async Task<string> Reference(string userId = null)
        {
            var command = new UserRoleCommand.Reference()
            {
                UserId = userId.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString("N") : userId
            };

            var roleDtos = new List<RoleDto>();
            await Enumerable.Range(0, 10)
                       .ForEachAsync(async s =>
                       {
                           roleDtos.Add(await this._roleController.RelationPermission());
                       });

            command.Ids = roleDtos.Select(s => s.Id).ToArray();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var userRoleDtos = await this.Get(command.UserId);

            Assert.True(userRoleDtos.All(s => command.Ids.Contains(s.RoleId)));

            return command.UserId;

        }

        [Fact]
        public async Task DeleteRole()
        {
            var userId = await this.Reference();
            var userRoles = await this.Get(userId);
            var command = new UserRoleCommand.DeleteReference
            {
                UserId = userId,
                Ids = new[] { userRoles.First().Id }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var userRoleDtos = await this.Get(command.UserId);

            Assert.True(!userRoleDtos.Any(s => command.Ids.Contains(s.Id)));
        }



        private async Task<IEnumerable<UserRoleDto>> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<IEnumerable<UserRoleDto>>();
        }
    }
}
