using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Domains;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.BasicData.Test
{
    public class LabelControllerTest : BaseTest
    {
        public const string API = "label";
        public LabelControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        public async Task<LabelDto> Add()
        {
            var command = new LabelCommand.Create
            {
                Name = this.GetRandom(),
                Creator = this.GetRandom(),
                Value = this.GetRandom()
            };

            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var label = await this.Get(id);
            Assert.Equal(command.Name, label.Name);
            // Assert.Equal(command.Creator, label.Creator);
            Assert.Equal(command.Value, label.Value);

            return label;
        }


        [Fact]
        public async Task Delete()
        {
            var labels = new List<LabelDto>();

            await Enumerable.Range(100, 1000)
                    .ForEachAsync(async s =>
                    {
                        labels.Add(await this.Add());
                    });

            foreach (var label in labels)
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{label.Id}");

                var responseMessage = await this.HttpClient.SendAsync(message);
                responseMessage.EnsureSuccessStatusCode();
                var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(label.Id));
                Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);
            }

        }

        private async Task<LabelDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            var label = await responseMessage.AsAsync<LabelDto>();

            this.WriteLine(label);

            return label;
        }
    }
}
