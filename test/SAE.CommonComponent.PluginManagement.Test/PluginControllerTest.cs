using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Domians;
using SAE.CommonComponent.PluginManagement.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.PluginManagement.Test
{
    public class PluginControllerTest : BaseTest
    {
        public const string API = "Plugin";
        private readonly IMediator _mediator;
        public PluginControllerTest(ITestOutputHelper output) : base(output)
        {
            this._mediator = this.ServiceProvider.GetService<IMediator>();
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<PluginDto> Add()
        {
            var command = new PluginCommand.Create
            {
                Name = $"test_{this.GetRandom()}",
                Description = this.GetRandom(),
                Order = (Math.Abs(this.GetRandom().GetHashCode() % 100)),
                Status = Status.Enable,
                Version = (Math.Abs(this.GetRandom().GetHashCode() % 100)).ToString()
            };

            var id =await this._mediator.SendAsync<string>(command);

            var plugin = await this.Get(id);

            Assert.Equal(plugin.Name, command.Name);
            Assert.Equal(plugin.Description, command.Description);
            Assert.Equal(plugin.Order, command.Order);
            Assert.Equal(plugin.Status, command.Status);
            Assert.Equal(plugin.Version, command.Version);
            this.WriteLine(plugin);
            return plugin;
        }
        [Fact]
        public async Task Edit()
        {
            var dto = await this.Add();
            var command = new PluginCommand.Change
            {
                Id = dto.Id,
                Description = this.GetRandom(),
                Order = (Math.Abs(this.GetRandom().GetHashCode() % 100)),
                Version = dto.Version + 1
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var plugin = await this.Get(dto.Id);

            Assert.Equal(plugin.Description, command.Description);
            Assert.Equal(plugin.Order, command.Order);
            Assert.Equal(plugin.Version, command.Version);
            this.WriteLine(plugin);
        }

        [Fact]
        public async Task ChangeStatus()
        {
            var dto = await this.Add();
            var command = new PluginCommand.ChangeStatus
            {
                Id = dto.Id,
                Status = dto.Status == Status.Enable ? Status.Disable : Status.Enable
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}/status");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var plugin = await this.Get(dto.Id);

            Assert.Equal(plugin.Status, command.Status);

            this.WriteLine(plugin);
        }

        [Fact]
        public async Task ChangeEntry()
        {
            var dto = await this.Add();
            var command = new PluginCommand.ChangeEntry
            {
                Id = dto.Id,
                Entry=$"http://sae.com/{this.GetRandom()}",
                Path=$"/{this.GetRandom()}"
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}/Entry");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var plugin = await this.Get(dto.Id);

            Assert.Equal(plugin.Entry, command.Entry);

            Assert.Equal(plugin.Path, command.Path);

            this.WriteLine(plugin);
        }

        [Fact]
        public async Task Delete()
        {
            var dto = await this.Add();

            var command = new Command.Delete<Plugin>
            {
                Id = dto.Id
            };


            await this._mediator.SendAsync(command);

            var exception = await Assert.ThrowsAnyAsync<SAEException>(() => this.Get(dto.Id));

            Assert.Equal(exception.Code, (int)StatusCodes.ResourcesNotExist);

        }

        
        private async Task<PluginDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<PluginDto>();
        }
    }
}
