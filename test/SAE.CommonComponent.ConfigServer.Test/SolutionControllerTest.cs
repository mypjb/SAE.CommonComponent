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
    public class SolutionControllerTest : ControllerTest
    {
        public SolutionControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        internal SolutionControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
        }

        public const string API = "solution";

        [Fact]
        public async Task<SolutionDto> Add()
        {
            var command = new SolutionCommand.Create
            {
                Name = this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var Solution = await this.Get(id);
            Assert.Equal(command.Name, Solution.Name);
            return Solution;
        }

        [Fact]
        public async Task Edit()
        {
            var Solution = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new SolutionCommand.Change
            {
                Id = Solution.Id,
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newSolution = await this.Get(Solution.Id);
            Assert.NotEqual(command.Name, Solution.Name);
            Assert.Equal(command.Name, newSolution.Name);
        }

        [Fact]
        public async Task Delete()
        {
            var Solution = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{Solution.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(Solution.Id));
            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
        }

        private async Task<SolutionDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<SolutionDto>();
        }

        
    }
}
