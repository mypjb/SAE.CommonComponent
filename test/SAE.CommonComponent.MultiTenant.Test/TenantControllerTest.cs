using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Application.Test;
using SAE.CommonComponent.MultiTenant.Commands;
using SAE.CommonComponent.MultiTenant.Domains;
using SAE.CommonComponent.MultiTenant.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.MultiTenant.Test
{
    public class TenantControllerTest : BaseTest
    {
        public const string API = "/Tenant";
        private readonly AppClusterControllerTest _appClusterController;

        public TenantControllerTest(ITestOutputHelper output) : base(output)
        {
            this._appClusterController = new AppClusterControllerTest(this._output);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>()
                          .ConfigureServices(s =>
                          {
                              s.AddSingleton(p => this._appClusterController.ServiceProvider.GetService<ICommandHandler<AppClusterCommand.Find, AppClusterDto>>());
                              s.AddSingleton(p => this._appClusterController.ServiceProvider.GetService<ICommandHandler<AppCommand.Create, string>>());
                          });
        }

        public async Task<TenantDto> Add(string parentId = null)
        {
            var command = new TenantCommand.Create
            {
                Name = this.GetRandom(),
                Description = this.GetRandom(),
                Domain = this.GetRandom(),
                Type = this.GetRandom(),
                ParentId = parentId
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var tenant = await this.Get(id);
            Assert.Equal(id, tenant.Id);
            Assert.Equal(command.Name, tenant.Name);
            Assert.Equal(command.Description, tenant.Description);
            Assert.Equal(command.Domain, tenant.Domain);
            Assert.Equal(command.Type, tenant.Type);
            return tenant;
        }
        [Fact]
        public async Task AppCreate()
        {
            var clusterDto = await this._appClusterController.Add();
            var tenant = await this.Add();

            var command = new TenantCommand.App.Create
            {
                Name = this.GetRandom(),
                Description = this.GetRandom(),
                Domain = this.GetRandom(),
                Type = clusterDto.Type
            };
            var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/app");
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var appId = await responseMessage.AsAsync<string>();
            var pagingReq = new HttpRequestMessage(HttpMethod.Get, $"{API}/app/paging?tenantId={tenant.Id}");
            var pagingRep = await this.HttpClient.SendAsync(pagingReq);
            var json = await pagingRep.Content.ReadAsStringAsync();
            this.WriteLine(json);
            // var appDtos = await responseMessage.AsAsync<PagedList<AppDto>>();
            // Assert.NotEmpty(appDtos);
            // Assert.True(appDtos.Count(s => s.Id == appId) == 1);
        }

        [Fact]
        public async Task Edit()
        {
            var tenant = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new TenantCommand.Change
            {
                Id = tenant.Id,
                Name = this.GetRandom(),
                Description = this.GetRandom(),
                Domain = this.GetRandom(),
                Type = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newTenant = await this.Get(tenant.Id);
            Assert.Equal(command.Name, newTenant.Name);
            Assert.Equal(command.Description, newTenant.Description);
            Assert.Equal(command.Domain, newTenant.Domain);
            Assert.Equal(command.Type, newTenant.Type);
        }


        [Fact]
        public async Task ChangeStatus()
        {
            var tenant = await this.Add();

            var command = new TenantCommand.ChangeStatus
            {
                Id = tenant.Id,
                Status = tenant.Status == Status.Enable ? Status.Disable : Status.Enable
            };

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/Status")
                                   .AddJsonContent(command);

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var dto = await this.Get(tenant.Id);

            Assert.Equal(command.Status, dto.Status);
        }


        [Fact]
        public async Task Delete()
        {
            var tenant = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{tenant.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(tenant.Id));
            Assert.Equal(exception.Code, (int)StatusCodes.ResourcesNotExist);
        }

        [Fact]
        public async Task Tree()
        {
            var parent = await this.Add();
            var child = await this.Add(parent.Id);
            var responseMessage = await this.HttpClient.GetAsync($"{API}/{nameof(Tree)}");
            var tenantItems = await responseMessage.AsAsync<IEnumerable<TenantItemDto>>();

            var parentTenant = tenantItems.First(s => s.Id == parent.Id);
            var childTenant = parentTenant.Items.First(s => s.Id == child.Id);
            Assert.Equal(parent.Name, parentTenant.Name);
            Assert.Equal(parent.ParentId, parentTenant.ParentId);
            Assert.Equal(parent.Description, parentTenant.Description);
            Assert.Equal(parent.CreateTime, parentTenant.CreateTime);
            Assert.Equal(child.Name, childTenant.Name);
            Assert.Equal(child.ParentId, childTenant.ParentId);
            Assert.Equal(child.Description, childTenant.Description);
            Assert.Equal(child.CreateTime, childTenant.CreateTime);
            this.WriteLine(tenantItems);
        }


        private async Task<TenantDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var tenant = await responseMessage.AsAsync<TenantDto>();
            this.WriteLine(tenant);
            return tenant;
        }
    }
}