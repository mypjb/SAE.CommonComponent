using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Test;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Domains;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Routing.Test
{
    public class MenuControllerTest : BaseTest
    {
        public const string API = "menu";
        private RoleControllerTest roleControllerTest;
        public MenuControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>()
                          .ConfigureServices(services =>
                          {
                              roleControllerTest = new RoleControllerTest(this._output);
                              var handlers = ServiceFacade.ServiceProvider.GetServices<ICommandHandler<PermissionCommand.Finds, IEnumerable<PermissionDto>>>();

                              services.AddSingleton(handlers);
                          });
        }

        [Theory]
        [InlineData(null)]
        public async Task<MenuDto> Add(string parentId = null)
        {
            var command = new MenuCommand.Create
            {
                Name = this.GetRandom(),
                ParentId = parentId,
                Path = $"/test/{this.GetRandom()}",
                Hidden = this.GetRandom().GetHashCode() % 2 == 0
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var menu = await this.Get(id);
            Assert.Equal(command.Name, menu.Name);
            Assert.Equal(command.Path, menu.Path);
            Assert.Equal(command.Hidden, menu.Hidden);

            return menu;
        }

        [Fact]
        public async Task Edit()
        {
            var menu = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new MenuCommand.Change
            {
                Id = menu.Id,
                Path = $"/test/{this.GetRandom()}",
                Name = this.GetRandom(),
                Hidden = !menu.Hidden
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newMenu = await this.Get(menu.Id);
            Assert.NotEqual(newMenu.Name, menu.Name);
            Assert.NotEqual(newMenu.Path, menu.Path);
            Assert.NotEqual(newMenu.Hidden, menu.Hidden);
        }

        [Fact]
        public async Task Delete()
        {
            var menu = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, API);
            message.AddJsonContent(new Command.BatchDelete<Menu>
            {
                Ids = new[] { menu.Id }
            });
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(menu.Id));
            Assert.Equal(exception.Code, (int)StatusCodes.ResourcesNotExist);
        }

        [Fact]
        public async Task Tree()
        {
            var parent = await this.Add();
            var child = await this.Add(parent.Id);
            var responseMessage = await this.HttpClient.GetAsync($"{API}/{nameof(Tree)}");
            var menus = await responseMessage.AsAsync<IEnumerable<MenuItemDto>>();

            var parentMenu = menus.First(s => s.Id == parent.Id);
            var childMenu = parentMenu.Items.First(s => s.Id == child.Id);
            Assert.Equal(parent.Name, parentMenu.Name);
            Assert.Equal(parent.ParentId, parentMenu.ParentId);
            Assert.Equal(parent.Path, parentMenu.Path);
            Assert.Equal(parent.Hidden, parentMenu.Hidden);
            Assert.Equal(child.Name, childMenu.Name);
            Assert.Equal(child.ParentId, childMenu.ParentId);
            Assert.Equal(child.Path, childMenu.Path);
            Assert.Equal(child.Hidden, childMenu.Hidden);
            this.WriteLine(menus);
        }

        [Fact]
        public async Task RelevancePermission()
        {

            var url = $"/{API}/permission";

            var roleDto = await roleControllerTest.RelevancePermission();

            var menu = await this.Add();
            var command = new MenuCommand.RelevancePermission
            {
                Id = menu.Id,
                PermissionIds = roleDto.PermissionIds.Take(10).ToList()
            };

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            req.AddJsonContent(command);

            var rep = await this.HttpClient.SendAsync(req);

            rep.EnsureSuccessStatusCode();

            var permissionQuery = new MenuCommand.PermissionQuery
            {
                Referenced = true,
                Id = menu.Id
            };

            var queryReq = new HttpRequestMessage(HttpMethod.Get, $"{url}/paging?{nameof(MenuCommand.PermissionQuery.Id)}={permissionQuery.Id}&{nameof(MenuCommand.PermissionQuery.Referenced)}={permissionQuery.Referenced}");

            var queryRep = await this.HttpClient.SendAsync(queryReq);

            var permissions = await queryRep.AsAsync<PagedList<PermissionDto>>();

            foreach (var permissionId in command.PermissionIds)
            {
                Assert.Contains(permissions, s => s.Id.Equals(permissionId));
            }

            var deleteCount = (Math.Abs(this.GetRandom().GetHashCode()) % (permissionQuery.PageSize - 1));

            deleteCount = deleteCount > 0 ? deleteCount : 1;

            var deleteCommand = new MenuCommand.DeletePermission
            {
                Id = menu.Id,
                PermissionIds = command.PermissionIds.Take(deleteCount)
            };

            var deleteReq = new HttpRequestMessage(HttpMethod.Delete, url);

            deleteReq.AddJsonContent(deleteCommand);

            var deleteRep = await this.HttpClient.SendAsync(deleteReq);

            deleteRep.EnsureSuccessStatusCode();

            queryReq = queryReq.Clone();

            queryRep = await this.HttpClient.SendAsync(queryReq);

            permissions = await queryRep.AsAsync<PagedList<PermissionDto>>();

            foreach (var permissionId in command.PermissionIds.Take(deleteCount))
            {
                Assert.DoesNotContain(permissions, s => s.Id == permissionId);
            }

            foreach (var permissionId in command.PermissionIds.Skip(deleteCount))
            {
                Assert.Contains(permissions, s => s.Id == permissionId);
            }

        }


        private async Task<MenuDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            return await responseMessage.AsAsync<MenuDto>();
        }
    }
}
