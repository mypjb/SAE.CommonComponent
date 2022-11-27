using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator.Behavior;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Authorize.Test
{
    public class RoleControllerTest : BaseTest
    {
        public const string API = "Role";
        private readonly PermissionControllerTest _permissionController;

        public RoleControllerTest(ITestOutputHelper output) : base(output)
        {
            this._permissionController = new PermissionControllerTest(output, this.HttpClient);
        }

        internal RoleControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._permissionController = new PermissionControllerTest(output, this.HttpClient);
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
                          }); ;
        }

        [Theory]
        [InlineData("")]
        public async Task<RoleDto> Add(string appId = null)
        {
            var command = new RoleCommand.Create
            {
                Name = $"test_{this.GetRandom()}",
                Description = "add Role",
                AppId = appId.IsNullOrWhiteSpace() ? Guid.Empty.ToString("N") : appId
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var id = await httpResponse.AsAsync<string>();
            var Role = await this.Get(id);

            Assert.Equal(Role.Name, command.Name);
            Assert.Equal(Role.Description, command.Description);
            this.WriteLine(Role);
            return Role;
        }
        [Fact]
        public async Task Edit()
        {
            var dto = await this.Add();
            var command = new RoleCommand.Change
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = "edit Role"
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");
            request.AddJsonContent(command);
            await this.HttpClient.SendAsync(request);
            var Role = await this.Get(dto.Id);

            Assert.Equal(Role.Name, command.Name);
            Assert.Equal(Role.Description, command.Description);
            this.WriteLine(Role);
        }

        [Fact]
        public async Task Delete()
        {
            var dto = await this.Add();

            var command = new Command.BatchDelete<Role>
            {
                Ids = new[] { dto.Id }
            };
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var exception = await Assert.ThrowsAnyAsync<SAEException>(() => this.Get(dto.Id));

            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);

        }

        [Fact]
        public async Task<RoleDto> ReferencePermission()
        {
            var roleDto = await this.Add();
            var permissionDtos = new List<PermissionDto>();
            await Enumerable.Range(0, 10)
                       .ForEachAsync(async s =>
                       {
                           permissionDtos.Add(await this._permissionController.Add());
                       });

            var command = new RoleCommand.ReferencePermission
            {
                Id = roleDto.Id,
                PermissionIds = permissionDtos.Select(s => s.Id).ToArray()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/Permission");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var role = await this.Get(roleDto.Id);

            var permissions = await GetPermission(role, true);

            Assert.True(permissions.All(s => command.PermissionIds.Contains(s.Id)));
            return role;

        }

        private async Task<IEnumerable<PermissionDto>> GetPermission(RoleDto role, bool referenced)
        {
            var rolePermissionRequest = new HttpRequestMessage(HttpMethod.Get, $"{API}/Permission/paging?Referenced={referenced}&id={role.Id}");

            var rolePermissionResponse = await this.HttpClient.SendAsync(rolePermissionRequest);

            var permissions = await rolePermissionResponse.AsAsync<PagedList<PermissionDto>>();
            return permissions;
        }

        [Fact]
        public async Task DeletePermission()
        {
            var roleDto = await this.ReferencePermission();

            var command = new RoleCommand.ReferencePermission
            {
                Id = roleDto.Id,
                PermissionIds = roleDto.PermissionIds.ToArray()
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}/permission");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var permissions = await GetPermission(roleDto, true);

            Assert.True(!permissions.Any(s => command.PermissionIds.Contains(s.Id)));
        }
        [Fact]
        public async Task BatchAdd()
        {
            var appId = this.GetRandom();
            var range = new Random().Next(100, 500);
            Enumerable.Range(0, range)
                      .AsParallel()
                      .ForAll(s =>
                      {
                          this.Add(appId).GetAwaiter().GetResult();
                      });
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/list?AppId={appId}");

            var responseMessage = await this.HttpClient.SendAsync(message);
            var resources = await responseMessage.AsAsync<RoleDto[]>();

            Assert.NotEmpty(resources);
            Assert.Equal(range, resources.Length);
            foreach (var resource in resources)
            {
                Assert.NotEqual(0, resource.Index);
                Assert.Equal(1, resources.Count(s => s.Index == resource.Index));
            }
        }

        private async Task<RoleDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<RoleDto>();
        }

    }
}
