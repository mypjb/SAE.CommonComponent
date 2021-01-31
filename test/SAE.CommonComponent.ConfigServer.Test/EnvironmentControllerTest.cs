using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Test.Models;
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
    public class EnvironmentControllerTest : ControllerTest
    {
        public const string API = "env";
        private readonly bool _depend;
        public const string DefaultEnv = "Development";
        public EnvironmentControllerTest(ITestOutputHelper output) : base(output)
        {
            this._depend = false;
        }

        internal EnvironmentControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._depend = true;
        }

        [Fact]
        public async Task<EnvironmentVariableDto> Add()
        {
            var command = new EnvironmentVariableCommand.Create
            {
                Name =this._depend? DefaultEnv : this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);

            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var EnvironmentVariable = await this.Get(id);
            Assert.Equal(command.Name, EnvironmentVariable.Name);

            return EnvironmentVariable;
        }

        [Fact]
        public async Task Edit()
        {
            var EnvironmentVariable = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new EnvironmentVariableCommand.Change
            {
                Id = EnvironmentVariable.Id,
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newEnvironmentVariable = await this.Get(EnvironmentVariable.Id);
            Assert.NotEqual(command.Name, EnvironmentVariable.Name);
            Assert.Equal(command.Name, newEnvironmentVariable.Name);
        }

        [Fact]
        public async Task Delete()
        {
            var EnvironmentVariable = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{EnvironmentVariable.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(EnvironmentVariable.Id));
            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
        }

        private async Task<EnvironmentVariableDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);

            return await responseMessage.AsAsync<EnvironmentVariableDto>();

        }
    }
}
