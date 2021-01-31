using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
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
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<RoleDto> Add()
        {
            var command = new RoleCommand.Create
            {
                Name = $"test_{this.GetRandom()}",
                Descriptor = "add Role"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var id = await httpResponse.AsAsync<string>();
            var Role = await this.Get(id);

            Assert.Equal(Role.Name, command.Name);
            Assert.Equal(Role.Descriptor, command.Descriptor);
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
                Name = $"edit_{this.GetRandom()}",
                Descriptor = "edit Role",
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var Role = await this.Get(dto.Id);

            Assert.Equal(Role.Name, command.Name);
            Assert.Equal(Role.Descriptor, command.Descriptor);
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

            Assert.Equal(exception.Code, (int)StatusCodes.ResourcesNotExist);

        }

        [Fact]
        public async Task<RoleDto> RelationPermission()
        {
            var roleDto = await this.Add();
            var permissionDtos = new List<PermissionDto>();
            await Enumerable.Range(0, 10)
                       .ForEachAsync(async s =>
                       {
                           permissionDtos.Add(await this._permissionController.Add());
                       });

            var command = new RoleCommand.RelationPermission
            {
                Id = roleDto.Id,
                PermissionIds = permissionDtos.Select(s => s.Id).ToArray()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(RelationPermission)}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var role = await this.Get(roleDto.Id);

            Assert.True(role.PermissionIds.All(s => command.PermissionIds.Contains(s)));

            return role;

        }

        [Fact]
        public async Task DeletePermission()
        {
            var roleDto = await this.RelationPermission();

            var command = new RoleCommand.RelationPermission
            {
                Id = roleDto.Id,
                PermissionIds = new[] { roleDto.PermissionIds.First() }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{nameof(RelationPermission)}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var role = await this.Get(roleDto.Id);

            Assert.True(!role.PermissionIds.Any(s => command.PermissionIds.Contains(s)));
        }



        private async Task<RoleDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<RoleDto>();
        }
    }
}
