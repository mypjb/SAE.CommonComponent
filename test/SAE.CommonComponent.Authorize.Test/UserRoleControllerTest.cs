using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Abstract.Mediator.Behavior;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;

namespace SAE.CommonComponent.Authorize.Test
{
    public class UserRoleControllerTest : BaseTest
    {
        public const string API = "/User/Role";
        private readonly PermissionControllerTest _permissionController;
        private readonly RoleControllerTest _roleController;

        public UserRoleControllerTest(ITestOutputHelper output) : base(output)
        {
            this._permissionController = new PermissionControllerTest(output, this.HttpClient);
            this._roleController = new RoleControllerTest(output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>().ConfigureAppConfiguration(c =>
                          {
                              c.AddInMemoryCollection(new Dictionary<string, string>()
                                                     {
                                                        {$"{RetryPipelineBehaviorOptions.Option}:{nameof(RetryPipelineBehaviorOptions.Num)}","10"}
                                                     });
                          })
                          .ConfigureServices(s =>
                          {
                              s.AddMediatorBehavior()
                               .AddRetry<RoleCommand.SetIndex>();
                              s.AddSingleton(p => this._roleController._appResourceController.ServiceProvider.GetService<ICommandHandler<AppResourceCommand.List, IEnumerable<AppResourceDto>>>());
                          });
        }

        [Theory]
        [InlineData(null)]
        public async Task<string> Reference(string userId = null)
        {
            var command = new UserRoleCommand.ReferenceRole()
            {
                UserId = userId.IsNullOrWhiteSpace() ? this.GetRandom() : userId
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

            var roles = await this.Get(command.UserId);

            Assert.True(roles.All(s => command.RoleIds.Contains(s.Id)));

            return command.UserId;

        }

        [Fact]
        public async Task DeleteRole()
        {
            var userId = await this.Reference();
            var userRoles = await this.Get(userId);
            var command = new UserRoleCommand.DeleteRole
            {
                UserId = userId,
                RoleIds = new[] { userRoles.First().Id }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var roles = await this.Get(command.UserId);

            Assert.True(!roles.Any(s => command.RoleIds.Contains(s.Id)));
        }



        private async Task<IEnumerable<RoleDto>> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/list?UserId={id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<RoleDto[]>();
        }
    }
}
