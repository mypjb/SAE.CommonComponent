using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.BasicData.Test;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using System.Collections.Generic;
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
        public const string API = "Project";
        private DictControllerTest _dictControllerTest;
        private ConfigControllerTest _configController;
        private SolutionControllerTest _solutionController;
        private readonly string _environmentId;
        public ProjectControllerTest(ITestOutputHelper output) : base(output)
        {
            this._dictControllerTest = new DictControllerTest(this._output);
            var dictDto = this._dictControllerTest.Add(null, (int)DictType.Environment).GetAwaiter().GetResult();
            this._environmentId = dictDto.Id;
            this._configController = new ConfigControllerTest(this._output, this.HttpClient, dictDto.Id);
            this._solutionController = new SolutionControllerTest(this._output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return base.UseStartup(builder).ConfigureServices(services =>
            {
                services.AddSingleton(p =>
                {
                    return this._dictControllerTest.ServiceProvider.GetServices<ICommandHandler<DictCommand.List, IEnumerable<DictDto>>>();
                });
            });
        }

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
            responseMessage.EnsureSuccessStatusCode();
            var id = await responseMessage.AsAsync<string>();
            var project = await this.Get(id);
            Assert.Equal(command.Name, project.Name);
            return project;
        }
        [Fact]
        public async Task Reference()
        {
            var config = await this._configController.Add();
            var project = await this.Add(config.SolutionId);
            var command = new ProjectCommand.ReferenceConfig
            {
                ProjectId = project.Id,
                ConfigIds = new[] { config.Id }
            };

            var url = $"{API}/config";

            var message = new HttpRequestMessage(HttpMethod.Post, url)
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            httpResponseMessage.EnsureSuccessStatusCode();

            var responseMessage = await this.HttpClient.GetAsync($"{API}/config/paging?{nameof(command.ProjectId)}={command.ProjectId}&&{nameof(config.EnvironmentId)}={config.EnvironmentId}");

            var projectConfigDtos = await responseMessage.AsAsync<PagedList<ProjectConfigDto>>();
            var configDto = projectConfigDtos.First().Config;

            Assert.Equal(configDto.Id, config.Id);
            Assert.Equal(configDto.SolutionId, config.SolutionId);
            Assert.Equal(configDto.EnvironmentId, config.EnvironmentId);
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
            responseMessage.EnsureSuccessStatusCode();
            var newProject = await this.Get(project.Id);
            Assert.NotEqual(newProject.Name, project.Name);
        }

        [Fact]
        public async Task Delete()
        {
            var project = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{project.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(project.Id));
            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
        }

        [Fact]
        public async Task AppConfig()
        {
            var config = await this._configController.Add();
            var publicConfig = await this._configController.Add(config.SolutionId);
            var project = await this.Add(config.SolutionId);
            var command = new ProjectCommand.ReferenceConfig
            {
                ProjectId = project.Id,
                ConfigIds = new[] { config.Id, publicConfig.Id }
            };

            var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/config")
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            var projectConfigsRep = await this.HttpClient.GetAsync($"{API}/config/paging?ProjectId={command.ProjectId}&EnvironmentId={config.EnvironmentId}");

            var projectConfigs = await projectConfigsRep.AsAsync<PagedList<ProjectConfigDto>>();

            var publicProjectConfig = projectConfigs.First(s => s.ConfigId == publicConfig.Id);

            var configChangeCommand = new ProjectCommand.ConfigChange
            {
                Id = publicProjectConfig.Id,
                Alias = this.GetRandom(),
                Private = false
            };

            var configChangeReq = new HttpRequestMessage(HttpMethod.Put, $"{API}/config");

            configChangeReq.AddJsonContent(configChangeCommand);

            (await this.HttpClient.SendAsync(configChangeReq)).EnsureSuccessStatusCode();

            var publishCommand = new ProjectCommand.Publish
            {
                Id = project.Id,
                EnvironmentId = config.EnvironmentId
            };

            var publishReq = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(ProjectCommand.Publish)}")
                              .AddJsonContent(publishCommand);

            (await this.HttpClient.SendAsync(publishReq)).EnsureSuccessStatusCode();

            var publishReq2 = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(ProjectCommand.Publish)}")
                             .AddJsonContent(publishCommand);

            (await this.HttpClient.SendAsync(publishReq2)).EnsureSuccessStatusCode();

            var previewCommand = new ProjectCommand.Preview
            {
                Id = project.Id,
                EnvironmentId = this._environmentId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{API}/Preview?{nameof(previewCommand.Id)}={previewCommand.Id}&{nameof(previewCommand.EnvironmentId)}={previewCommand.EnvironmentId}");

            var responseMessage = await this.HttpClient.SendAsync(requestMessage);

            var projectPreview = await responseMessage.AsAsync<ProjectPreviewDto>();
            this.WriteLine(new
            {
                Source = new { Public = publicConfig, Private = config},
                New = projectPreview
            });
            Assert.Contains(config.Content, projectPreview.Private.ToJsonString());
            Assert.Contains(publicConfig.Content, projectPreview.Public.ToJsonString());
        }

        private async Task<ProjectDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            return await responseMessage.AsAsync<ProjectDto>();
        }

    }
}
