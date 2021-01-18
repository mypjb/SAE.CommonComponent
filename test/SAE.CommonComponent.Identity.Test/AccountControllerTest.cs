using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Authorize.Test;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.Test;
using SAE.CommonComponent.User.Abstract.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
using SAE.CommonLibrary.ObjectMapper;
using SAE.CommonComponent.Authorize.Commands;

namespace SAE.CommonComponent.Identity.Test
{
    public class AccountControllerTest : BaseTest
    {
        public const string API = "account";
        private UserControllerTest _userController;
        private UserRoleControllerTest _userRoleController;

        public AccountControllerTest(ITestOutputHelper output) : base(output)
        {
            this._userRoleController = new UserRoleControllerTest(this._output);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
           
            return builder.ConfigureServices(services =>
            {
                this._userController = new UserControllerTest(this._output);
                
                var authenticationHandlers = ServiceFacade.ServiceProvider.GetServices<ICommandHandler<UserCommand.Authentication, UserDto>>();
                services.AddSingleton(authenticationHandlers);
                
            }).UseStartup<Startup>();
        }

        [Fact]
        public async Task Login()
        {
            var user = await this._userController.Register();

            var userId = await this._userRoleController.Reference(user.Id);


            var command = new AccountLoginCommand
            {
                Remember = true,
                Name = user.Name,
                Password = UserControllerTest.DefaultPassword
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Login)}");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            Assert.True(httpResponse.IsSuccessStatusCode);

            this.WriteLine(httpResponse.Headers);

        }
    }
}
