using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Mediator.Behavior;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Authorize.Test
{
    public class ClientRoleControllerTest : BaseTest
    {
        public const string API = "/client/role";
        private readonly RoleControllerTest _roleController;

        public ClientRoleControllerTest(ITestOutputHelper output) : base(output)
        {
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
        public async Task<string> Reference(string clientId = null)
        {
            var command = new ClientRoleCommand.ReferenceRole()
            {
                ClientId = clientId.IsNullOrWhiteSpace() ? this.GetRandom() : clientId
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
            var clientId = await this.Reference();
            var clientRoles = await this.Get(clientId);
            var command = new ClientRoleCommand.DeleteRole
            {
                ClientId = clientId,
                RoleIds = new[] { clientRoles.First().Id }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var roles = await this.Get(command.ClientId);

            Assert.True(!roles.Any(s => command.RoleIds.Contains(s.Id)));

            Assert.Contains(roles, s => clientRoles.Any(cr => cr.Id == s.Id));
        }



        private async Task<IEnumerable<RoleDto>> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/list?clientId={id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<RoleDto[]>();
        }
    }
}
