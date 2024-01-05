using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Test.Models;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.ConfigServer.Test
{
    public class TemplateControllerTest : ControllerTest
    {
        public const string API = "template";
        public TemplateControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        internal TemplateControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
        }

        public async Task<TemplateDto> Add()
        {
            var command = new TemplateCommand.Create
            {
                Name = this.GetRandom(),
                Format = this.GetConfig()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var template = await this.Get(id);
            Assert.Equal(command.Name, template.Name);
            Assert.Equal(command.Format, template.Format);
         
            return template;
        }

        [Fact]
        public async Task Edit()
        {
            var template=await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new TemplateCommand.Change
            {
                Id = template.Id,
                Format = this.GetConfig(),
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage= await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newTemplate= await this.Get(template.Id);
            Assert.NotEqual(command.Name, template.Name);
            Assert.NotEqual(command.Format, template.Format);
            Assert.Equal(command.Name, newTemplate.Name);
            Assert.Equal(command.Format, newTemplate.Format);
        }

        [Fact]
        public async Task Delete()
        {
            var template = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{template.Id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var exception= await Assert.ThrowsAsync<SAEException>(()=> this.Get(template.Id));
            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
        }

        private async Task<TemplateDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);

            return await responseMessage.AsAsync<TemplateDto>();

        }

        private string GetConfig()
        {
            return new CustomConfig
            {
                Basic = new BasicConifg
                {
                    Contact = new Contact
                    {
                        QQ = this.GetRandom(),
                        Tel = this.GetRandom(),
                        Wechat = this.GetRandom()
                    },
                    Name = this.GetRandom()
                }
            }.ToJsonString();
        }
    }
}
