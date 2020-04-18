using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Abstract.Commands;
using SAE.CommonComponent.Application.Abstract.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Application.Test
{
    public class ScopeControllerTest : BaseTest
    {
        public const string API = "scope";
        public ScopeControllerTest(ITestOutputHelper output) : base(output)
        {
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task<ScopeDto> Add()
        {
            var command = new ScopeCreateCommand
            {
                Name = this.GetRandom(),
                Display = this.GetRandom()
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            
            var scope = await this.Get(command.Name);

            Assert.Equal(command.Name, scope.Name);

            return scope;
        }


        [Fact]
        public async Task Delete()
        {
            var scope = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Delete, API);
            message.AddJsonContent(new ScopeRemoveCommand
            {
                Name = scope.Name
            });
            var responseMessage = await this.HttpClient.SendAsync(message);
            await responseMessage.AsResult();

            Assert.Null(await this.Get(scope.Name));
        }

       
        private async Task<ScopeDto> Get(string name)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/all");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var scopes = await responseMessage.AsResult<IEnumerable<ScopeDto>>();
            return scopes.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
