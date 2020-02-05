using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
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
            var command = new ProjectCreateCommand
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
            var command = new ProjectRelevanceConfigCommand
            {
                Id = project.Id,
                ConfigIds = new[] { config.Id }
            };
            var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Relevance)}")
                              .AddJsonContent(command);

            await this.HttpClient.SendAsync(message);

            var newProject = await this.Get(project.Id);

        }

        [Fact]
        public async Task Edit()
        {
            var project = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new ProjectChangeCommand
            {
                Id = project.Id,
                Name = this.GetRandom(),
                Version = project.Version + 1
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            await responseMessage.AsResult();
            var newProject = await this.Get(project.Id);
            Assert.NotEqual(command.Name, project.Name);
            Assert.NotEqual(command.Version, project.Version);
            Assert.Equal(command.Name, newProject.Name);
            Assert.Equal(command.Version, newProject.Version);
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
            var command = new ProjectRelevanceConfigCommand
            {
                Id = project.Id,
                ConfigIds = new[] { config.Id }
            };
            var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Relevance)}")
                              .AddJsonContent(command);

            await this.HttpClient.SendAsync(message);

            var appConfigCommand = new AppConfigCommand
            {
                Id = project.Id
            };

            var responseMessage = await this.HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"app/config")
                                                 .AddJsonContent(appConfigCommand));
            var appConfig =await responseMessage.AsResult<AppConfigDto>();

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
