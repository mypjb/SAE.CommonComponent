using SAE.CommonComponent.ConfigServer.Commands;
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
            this._configController = new ConfigControllerTest(this._output, this.HttpClient);
            this._solutionController = new SolutionControllerTest(output, this.HttpClient);
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
            responseMessage.EnsureSuccessStatusCode();
            var id = await responseMessage.AsAsync<string>();
            var project = await this.Get(id);
            Assert.Equal(command.Name, project.Name);
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

            var responseMessage = await this.HttpClient.GetAsync($"{url}/paging?{nameof(command.ProjectId)}={command.ProjectId}&&{nameof(config.EnvironmentId)}={config.EnvironmentId}");

            var configs = await responseMessage.AsAsync<PagedList<ConfigDto>>();
            var configDto = configs.First();

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
            var project = await this.Add(config.SolutionId);
            var command = new ProjectCommand.RelevanceConfig
            {
                ProjectId = project.Id,
                ConfigIds = new[] { config.Id }
            };


            var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/config/{nameof(Relevance)}")
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            var publishCommand = new ProjectCommand.Publish
            {
                Id = project.Id,
                EnvironmentId = config.EnvironmentId
            };

            var publishReq = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(ProjectCommand.Publish)}")
                              .AddJsonContent(publishCommand);

            (await this.HttpClient.SendAsync(publishReq)).EnsureSuccessStatusCode();

            var appConfigCommand = new AppCommand.Config
            {
                Id = project.Id,
                Env = EnvironmentControllerTest.DefaultEnv
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"app/config?{nameof(appConfigCommand.Id)}={appConfigCommand.Id}&{nameof(appConfigCommand.Env)}={appConfigCommand.Env}&{nameof(appConfigCommand.Version)}={appConfigCommand.Version}");

            var responseMessage = await this.HttpClient.SendAsync(requestMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
           
            Assert.Contains(config.Content, content);
        }

        private async Task<ProjectDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            return await responseMessage.AsAsync<ProjectDto>();
        }

    }
}
