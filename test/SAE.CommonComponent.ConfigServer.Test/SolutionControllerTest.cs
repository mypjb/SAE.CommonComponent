using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

       
        public const string API = "solution";

        [Fact]
        public async Task<Solution> Add()
        {
            var command = new SolutionCreateCommand
            {
                Name = this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsResult<string>();
            var Solution = await this.Get(id);
            Assert.Equal(command.Name, Solution.Name);
            return Solution;
        }

        [Fact]
        public async Task Edit()
        {
            var Solution = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new SolutionChangeCommand
            {
                Id = Solution.Id,
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            await responseMessage.AsResult();
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
            await responseMessage.AsResult();
            var exception = await Assert.ThrowsAsync<SaeException>(() => this.Get(Solution.Id));
            Assert.Equal(StatusCode.ResourcesNotExist, exception.Code);
        }

        private async Task<Solution> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsResult<Solution>();
        }

        
    }
}
