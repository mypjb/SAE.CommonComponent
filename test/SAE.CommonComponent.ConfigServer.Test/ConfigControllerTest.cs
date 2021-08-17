using SAE.CommonComponent.Application.Test;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.ConfigServer.Test
{
    public class ConfigControllerTest : ControllerTest
    {
        public const string API = "Config";
        private readonly TemplateControllerTest _templateController;
        private readonly AppClusterControllerTest _appClusterControllerTest;
        private readonly string _environmentId;
        public ConfigControllerTest(ITestOutputHelper output) : base(output)
        {
            this._templateController = new TemplateControllerTest(output, this.HttpClient);
            this._appClusterControllerTest= new AppClusterControllerTest(this._output);
            this._environmentId = Guid.NewGuid().ToString("N");
        }

        internal ConfigControllerTest(ITestOutputHelper output, HttpClient httpClient, string envId) : base(output, httpClient)
        {
            this._appClusterControllerTest = new AppClusterControllerTest(this._output);
            this._templateController = new TemplateControllerTest(this._output, this.HttpClient);
            this._environmentId = envId;
        }

        [Theory]
        [InlineData(null)]
        public async Task<ConfigDto> Add(string clusterId = null)
        {
            if (clusterId.IsNullOrWhiteSpace())
            {
                var clusterDto = await this._appClusterControllerTest.Add();
                clusterId = clusterDto.Id;
            }
            
            var template = await this._templateController.Add();
            var commond = new ConfigCommand.Create
            {
                Name = template.Name,
                Content = template.Format,
                ClusterId = clusterId,
                EnvironmentId = this._environmentId
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(commond);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var id = await responseMessage.AsAsync<string>();
            var config = await this.Get(id);
            Assert.Equal(commond.Name, config.Name);
            Assert.Equal(commond.Content, config.Content);
            Assert.Equal(commond.EnvironmentId, config.EnvironmentId);
            Assert.Equal(commond.ClusterId, config.ClusterId);
            return config;
        }

        [Fact]
        public async Task Edit()
        {
            var config = await this.Add();
            var template = await this._templateController.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var commond = new ConfigCommand.Change
            {
                Id = config.Id,
                Name = template.Name,
                Content = template.Format
            };
            message.AddJsonContent(commond);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newConfig = await this.Get(config.Id);
            Assert.NotEqual(commond.Name, config.Name);
            Assert.NotEqual(commond.Content, config.Content);
            Assert.Equal(commond.Name, newConfig.Name);
            Assert.Equal(commond.Content, newConfig.Content);
        }

        [Fact]
        public async Task Delete()
        {
            var config = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{config.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(config.Id));
            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
        }

        private async Task<ConfigDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);

            return await responseMessage.AsAsync<ConfigDto>();
        }


    }
}
