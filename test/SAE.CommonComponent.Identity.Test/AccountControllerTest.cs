using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.Test;
using SAE.CommonComponent.User.Abstract.Dtos;
using SAE.CommonComponent.User.Test;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SAE.CommonComponent.Identity.Test
{
    public class AccountControllerTest : BaseTest
    {
        public const string API = "account";
        private readonly UserControllerTest _userController;


        public AccountControllerTest(ITestOutputHelper output) : base(output)
        {
            this._userController = new UserControllerTest(this._output);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        [Fact]
        public async Task Login()
        {
            var user = await this._userController.Register();

            var command = new AccountLoginCommand
            {
                IsPersistent = true,
                Name = user.Name,
                Password = UserControllerTest.DefaultPassword
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Login)}");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            var dto =await httpResponse.AsAsync<UserDto>();

            Assert.NotNull(dto);
            Assert.Equal(dto.AccountName, user.AccountName);
            Assert.Equal(dto.Id, user.Id);
            Assert.Equal(dto.Status, user.Status);
            Assert.Equal(dto.CreateTime, user.CreateTime);
            Assert.Equal(dto.Name, user.Name);
            
        }
    }
}
