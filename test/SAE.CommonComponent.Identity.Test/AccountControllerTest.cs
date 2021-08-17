using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Authorize.Test;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.Test;
using SAE.CommonComponent.User.Dtos;
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
using System.Net;

namespace SAE.CommonComponent.Identity.Test
{
    public class AccountControllerTest : BaseTest
    {
        public const string API = "account";
        private UserControllerTest _userController;

        public AccountControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            this._userController = new UserControllerTest(this._output);

            return builder.ConfigureServices(services =>
            {
                var serviceProvider = this._userController.ServiceProvider;
                var authenticationHandlers = serviceProvider.GetServices<ICommandHandler<UserCommand.Authentication, UserDto>>();
                var createUserHandlers = serviceProvider.GetServices<ICommandHandler<UserCommand.Create, string>>();
                services.AddSingleton(authenticationHandlers);
                services.AddSingleton(authenticationHandlers);
                services.AddSingleton(createUserHandlers);
            }).UseStartup<Startup>();
        }
        [Fact]
        public async Task Login()
        {
            var password = this.GetRandom();
            var registerCommand = new AccountCommand.Register
            {
                Name = this.GetRandom(),
                Password = password,
                ConfirmPassword = password
            };

            var registerRequest = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(AccountCommand.Register)}");

            registerRequest.AddJsonContent(registerCommand);

            var registerResponse = await this.HttpClient.SendAsync(registerRequest);

            var userId = await registerResponse.AsAsync<string>();

#warning Annotating user roles
            //await this._userRoleController.Reference(userId);

            var command = new AccountCommand.Login
            {
                Remember = true,
                Name = registerCommand.Name,
                Password = registerCommand.Password
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(Login)}");

            var context = new MultipartFormDataContent();

            context.Add(new StringContent(command.Remember.ToString()), nameof(command.Remember));
            context.Add(new StringContent(command.Name), nameof(command.Name));
            context.Add(new StringContent(command.Password), nameof(command.Password));

            request.Content = context;

            var httpResponse = await this.HttpClient.SendAsync(request);

            Assert.True(httpResponse.StatusCode == HttpStatusCode.Found &&
                httpResponse.Headers.Location.ToString().Equals("/"));

            this.WriteLine(httpResponse.Headers);
        }
    }
}
