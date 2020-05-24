using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.ConfigServer.Test
{
    public class ConfigControllerTest : ControllerTest
    {
        public ConfigControllerTest(ITestOutputHelper output) : base(output)
        {
            this._templateController = new TemplateControllerTest(output);
            this._solutionController = new SolutionControllerTest(output);
            this._templateController.UseClient(this.HttpClient);
            this._solutionController.UseClient(this.HttpClient);
        }

        public const string API = "Config";
        private readonly TemplateControllerTest _templateController;
        private readonly SolutionControllerTest _solutionController;

        [Fact]
        public async Task<ConfigDto> Add()
        {
            var solution = await this._solutionController.Add();
            var template = await this._templateController.Add();
            var commond = new ConfigCommand.Create
            {
                Name = template.Name,
                Content = template.Format,
                SolutionId = solution.Id
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(commond);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var id = await responseMessage.AsAsync<string>();
            var config = await this.Get(id);
            Assert.Equal(commond.Name, config.Name);
            Assert.Equal(commond.Content, config.Content);
            Assert.Equal(commond.SolutionId, config.SolutionId);
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
            var exception = await Assert.ThrowsAsync<SaeException>(() => this.Get(config.Id));
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
