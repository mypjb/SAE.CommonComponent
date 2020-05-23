using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SAE.CommonComponent.Test;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Domains;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using SAE.CommonLibrary.Extension;
using SAE.CommonComponent.User.Abstract.Dtos;

namespace SAE.CommonComponent.User.Test
{
    public class UserControllerTest : BaseTest
    {
        public const string API = "user";
        public UserControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task Register()
        {
            var password = "Aa123456";
            var command = new UserCommand.Register
            {
                Name = $"test_{this.GetRandom().Replace("-", string.Empty)}",
                ConfirmPassword = password,
                Password = password
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Register)}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

        }

        private async Task<UserDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<UserDto>();
        }
    }
}
