using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Model;
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
                Name = dto.Name,
                Descriptor = "edit Role",
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");
            request.AddJsonContent(command);
            await this.HttpClient.SendAsync(request);
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
        public async Task<RoleDto> RelevancePermission()
        {
            var roleDto = await this.Add();
            var permissionDtos = new List<PermissionDto>();
            await Enumerable.Range(0, 10)
                       .ForEachAsync(async s =>
                       {
                           permissionDtos.Add(await this._permissionController.Add());
                       });

            var command = new RoleCommand.RelevancePermission
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
            var roleDto = await this.RelevancePermission();

            var command = new RoleCommand.RelevancePermission
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

        private async Task<RoleDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<RoleDto>();
        }

        public async Task DeleteMenu(string[] menuIds)
        {
            var roleDto = await this.RelevanceMenu(menuIds);

            var command = new RoleCommand.DeleteMenu
            {
                Id = roleDto.Id,
                MenuIds = roleDto.MenuIds.Skip(new Random().Next(1, menuIds.Length)).ToArray()
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}/menu");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var role = await this.Get(roleDto.Id);

            Assert.True(!role.MenuIds.Any(s => command.MenuIds.Contains(s)));
        }

        public async Task<RoleDto> RelevanceMenu(string[] menuIds)
        {
            var roleDto = await this.Add();

            var command = new RoleCommand.RelevanceMenu
            {
                Id = roleDto.Id,
                MenuIds = menuIds,
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/menu");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var role = await this.Get(roleDto.Id);

            Assert.True(role.MenuIds.All(s => command.MenuIds.Contains(s)));

            return role;
        }
    }
}
