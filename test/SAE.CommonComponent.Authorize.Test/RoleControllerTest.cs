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
using SAE.CommonComponent.Application.Test;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Mediator.Behavior;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Authorize.Test
{
    public class RoleControllerTest : BaseTest
    {
        public const string API = "Role";
        private readonly PermissionControllerTest _permissionController;
        protected readonly AppResourceControllerTest _appResourceController;
        private readonly IBitmapAuthorization _bitmapAuthorization;

        public RoleControllerTest(ITestOutputHelper output) : base(output)
        {
            this._permissionController = new PermissionControllerTest(output, this.HttpClient);
            this._appResourceController = new AppResourceControllerTest(output);
            this._bitmapAuthorization = this.ServiceProvider.GetService<IBitmapAuthorization>();
            this.SwitchContext(this.ServiceProvider);
        }

        internal RoleControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._permissionController = new PermissionControllerTest(output, this.HttpClient);
            this._appResourceController = new AppResourceControllerTest(output);
            this._bitmapAuthorization = this.ServiceProvider.GetService<IBitmapAuthorization>();
            this.SwitchContext(this.ServiceProvider);
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
                              s.AddSingleton(p => this._appResourceController.ServiceProvider.GetService<ICommandHandler<AppResourceCommand.List, IEnumerable<AppResourceDto>>>());
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
                AppId = appId.IsNullOrWhiteSpace() ? this.GetRandom() : appId
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
                           this.SwitchContext(this._appResourceController.ServiceProvider);
                           var appResourceDto = await this._appResourceController.Add(roleDto.AppId);
                           this.SwitchContext(this.ServiceProvider);
                           permissionDtos.Add(await this._permissionController.Add(roleDto.AppId, appResourceDto.Id));
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

            var permissions = await GetPermission(role);

            Assert.True(permissions.All(s => command.PermissionIds.Contains(s.Id)));
            
            var appResourceListCommand = new AppResourceCommand.List
            {
                AppId = role.AppId
            };

            var mediator = this._appResourceController.ServiceProvider.GetService<IMediator>();

            var appResources = await mediator.SendAsync<IEnumerable<AppResourceDto>>(appResourceListCommand);

            foreach (var resource in appResources)
            {
                Assert.True(this._bitmapAuthorization.Authorize(role.PermissionCode, resource.Index));
            }

            return role;

        }

        private async Task<IEnumerable<PermissionDto>> GetPermission(RoleDto role)
        {
            return role.Permissions;
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

            var permissions = await GetPermission(roleDto);

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
