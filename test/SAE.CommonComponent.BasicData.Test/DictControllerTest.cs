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
    public class DictControllerTest : BaseTest
    {
        public const string API = "dict";
        public DictControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Theory]
        [InlineData(null, 0)]
        public async Task<DictDto> Add(string parentId = null, int type = 0)
        {
            var command = new DictCommand.Create
            {
                Name = this.GetRandom(),
                ParentId = parentId,
                Type = type > 0 ? type : Math.Abs(this.GetRandom().GetHashCode() / 100)
            };

            if (!parentId.IsNullOrWhiteSpace())
            {
                var parentDict = await this.Get(parentId);
                command.Type = parentDict.Type;
            }

            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var Dict = await this.Get(id);
            Assert.Equal(command.Name, Dict.Name);

            return Dict;
        }

        [Fact]
        public async Task Edit()
        {
            var Dict = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new DictCommand.Change
            {
                Id = Dict.Id,
                Name = this.GetRandom()
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newDict = await this.Get(Dict.Id);
            Assert.NotEqual(newDict.Name, Dict.Name);
        }

        [Fact]
        public async Task Delete()
        {
            var dicts = await this.Tree();

            foreach (var dict in dicts)
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"{API}/{dict.Id}");

                var responseMessage = await this.HttpClient.SendAsync(message);
                responseMessage.EnsureSuccessStatusCode();
                var exception = await Assert.ThrowsAsync<SAEException>(() => this.Get(dict.Id));
                Assert.Equal(exception.Code, (int)StatusCodes.ResourcesNotExist);

                foreach (var item in dict.Items)
                {
                    var itemException = await Assert.ThrowsAsync<SAEException>(() => this.Get(item.Id));
                    Assert.Equal(itemException.Code, (int)StatusCodes.ResourcesNotExist);
                }

            }

        }

        [Fact]
        public async Task<IEnumerable<DictItemDto>> Tree()
        {
            var parent = await this.Add();
            var child = await this.Add(parent.Id);
            var responseMessage = await this.HttpClient.GetAsync($"{API}/{nameof(Tree)}?id={parent.ParentId}");
            var dicts = await responseMessage.AsAsync<IEnumerable<DictItemDto>>();

            var parentDict = dicts.First(s => s.Id == parent.Id);
            var childDict = parentDict.Items.First(s => s.Id == child.Id);
            Assert.Equal(parent.Name, parentDict.Name);
            Assert.Equal(parent.ParentId, parentDict.ParentId);
            Assert.Equal(parent.Type, parentDict.Type);
            Assert.Equal(child.Name, childDict.Name);
            Assert.Equal(child.ParentId, childDict.ParentId);
            Assert.Equal(child.Type, childDict.Type);
            this.WriteLine(dicts);

            return dicts;
        }

        private async Task<DictDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            var dict = await responseMessage.AsAsync<DictDto>();

            this.WriteLine(dict);

            return dict;
        }
    }
}
