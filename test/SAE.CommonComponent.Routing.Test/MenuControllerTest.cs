using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Domains;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
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
        public MenuControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Theory]
        [InlineData(null)]
        public async Task<MenuDto> Add(string parentId = null)
        {
            var command = new MenuCommand.Create
            {
                Name = this.GetRandom(),
                ParentId = parentId,
                Path = $"/test/{this.GetRandom()}"
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var menu = await this.Get(id);
            Assert.Equal(command.Name, menu.Name);
            Assert.Equal(command.Path, menu.Path);

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
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newMenu = await this.Get(menu.Id);
            Assert.NotEqual(newMenu.Name, menu.Name);
            Assert.NotEqual(newMenu.Path, menu.Path);
        }

        [Fact]
        public async Task Delete()
        {
            var menu = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, API);
            message.AddJsonContent(new BatchRemoveCommand<Menu>
            {
                Ids = new[] { menu.Id }
            });
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SaeException>(() => this.Get(menu.Id));
            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
        }

        [Fact]
        public async Task ALL()
        {
            var parent= await this.Add();
            var child = await this.Add(parent.Id);
            var responseMessage = await this.HttpClient.GetAsync($"{API}/{nameof(ALL)}");
            var menus = await responseMessage.AsAsync<IEnumerable<MenuItemDto>>();

            var parentMenu = menus.First(s => s.Id == parent.Id);
            var childMenu = parentMenu.Items.First(s => s.Id == child.Id);
            Assert.Equal(parent.Name, parentMenu.Name);
            Assert.Equal(parent.ParentId, parentMenu.ParentId);
            Assert.Equal(parent.Path, parentMenu.Path);

            Assert.Equal(child.Name,childMenu.Name);
            Assert.Equal(child.ParentId, childMenu.ParentId);
            Assert.Equal(child.Path, childMenu.Path);
            this.WriteLine(menus);
        }

        private async Task<MenuDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<MenuDto>();
        }
    }
}
