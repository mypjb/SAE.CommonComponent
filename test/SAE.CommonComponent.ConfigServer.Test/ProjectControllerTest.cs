﻿using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.ConfigServer.Test
{
    public class ProjectControllerTest : ControllerTest
    {
        public ProjectControllerTest(ITestOutputHelper output) : base(output)
        {
            this._configController = new ConfigControllerTest(this._output);
            this._solutionController = new SolutionControllerTest(output);
            this._configController.UseClient(this.HttpClient);
            this._solutionController.UseClient(this.HttpClient);
        }

        public const string API = "Project";
        private readonly ConfigControllerTest _configController;
        private readonly SolutionControllerTest _solutionController;

        [Theory]
        [InlineData("")]
        public async Task<ProjectDto> Add(string solutionId = "")
        {
            if (solutionId.IsNullOrWhiteSpace())
            {
                var solution = await this._solutionController.Add();
                solutionId = solution.Id;
            }
            var command = new ProjectCommand.Create
            {
                Name = this.GetRandom(),
                SolutionId = solutionId
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsResult<string>();
            var project = await this.Get(id);
            Assert.Equal(command.Name, project.Name);
            Assert.Equal(command.Version, project.Version);
            return project;
        }
        [Fact]
        public async Task Relevance()
        {
            var config = await this._configController.Add();
            var project = await this.Add(config.SolutionId);
            var command = new ProjectCommand.RelevanceConfig
            {
                ProjectId = project.Id,
                ConfigIds = new[] { config.Id }
            };

            var url = $"{API}/config/{nameof(Relevance)}";

            var message = new HttpRequestMessage(HttpMethod.Post, url)
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            var responseMessage = await this.HttpClient.GetAsync($"{url}?{nameof(command.ProjectId)}={command.ProjectId}");

            var configs = await responseMessage.AsResult<PagedList<ConfigDto>>();
            var configDto = configs.First();

            Assert.Equal(configDto.Id, config.Id);
            Assert.Equal(configDto.SolutionId, config.SolutionId);
            Assert.Equal(configDto.Content, config.Content);
            Assert.Equal(configDto.Version, config.Version);
        }

        [Fact]
        public async Task Edit()
        {
            var project = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new ProjectCommand.Change
            {
                Id = project.Id,
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            await responseMessage.AsResult();
            var newProject = await this.Get(project.Id);
            Assert.NotEqual(newProject.Name, project.Name);
        }

        [Fact]
        public async Task Delete()
        {
            var project = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{project.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            await responseMessage.AsResult();
            var exception = await Assert.ThrowsAsync<SaeException>(() => this.Get(project.Id));
            Assert.Equal(StatusCode.ResourcesNotExist, exception.Code);
        }

        [Fact]
        public async Task AppConfig()
        {
            var config = await this._configController.Add();
            var project = await this.Add(config.SolutionId);
            var command = new ProjectCommand.RelevanceConfig
            {
                ProjectId = project.Id,
                ConfigIds = new[] { config.Id }
            };
            var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/config/{nameof(Relevance)}")
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            var appConfigCommand = new AppCommand.Config
            {
                Id = project.Id
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"app/config?{nameof(appConfigCommand.Id)}={appConfigCommand.Id}&{appConfigCommand.Version}={appConfigCommand.Version}");

            var responseMessage = await this.HttpClient.SendAsync(requestMessage);
            var appConfig = await responseMessage.AsResult<AppConfigDto>();

            Assert.True(appConfig.Data.Count == 1);
            Assert.Equal(config.Content, appConfig.Data.First().Value.ToJsonString());
        }

        private async Task<ProjectDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsResult<ProjectDto>();
        }

    }
}
