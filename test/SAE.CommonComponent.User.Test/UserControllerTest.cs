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
using SAE.CommonComponent.User.Dtos;
using Assert = Xunit.Assert;
using SAE.CommonComponent.Authorize.Test;

namespace SAE.CommonComponent.User.Test
{
    public class UserControllerTest : BaseTest
    {
        public const string API = "user";
        public const string DefaultPassword = "Aa123456";
        public UserControllerTest(ITestOutputHelper output) : base(output)
        {
            
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }
        public async Task<UserDto> Add()
        {
            var command = new UserCommand.Create
            {
                Name = $"test_{this.GetRandom().Replace("-", string.Empty)}",
                Password = DefaultPassword
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var id = await httpResponse.AsAsync<string>();
            var user = await this.Get(id);

            Assert.Equal(user.Name, command.Name);
            this.WriteLine(user);
            return user;
        }


        [Fact]
        public async Task Password()
        {
            var password = this.GetRandom();
            var dto = await this.Add();

            var command = new UserCommand.ChangePassword
            {
                Id = dto.Id,
                ConfirmPassword = password,
                Password = password,
                OriginalPassword = DefaultPassword
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}/{nameof(Password)}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);

        }

        [Fact]
        public async Task Status()
        {
            var password = this.GetRandom();
            var dto = await this.Add();

            var command = new UserCommand.ChangeStatus
            {
                Id = dto.Id,
                Status = 1 - (int)CommonComponent.Status.Enable
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}/{nameof(Status)}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var user = await this.Get(dto.Id);
            this.WriteLine(user);
            Xunit.Assert.Equal(command.Status, user.Status);
        }

        private async Task<UserDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<UserDto>();
        }
    }
}
