using Microsoft.VisualBasic;
using IdentityModel;
using IdentityServer4;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.User.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SAE.CommonComponent.Identity.Handlers
{
    public class AccountHandler : ICommandHandler<AccountCommand.Login, IPrincipal>,
                                  ICommandHandler<AccountCommand.Register,string>
    {
        private readonly IMediator _mediator;

        public AccountHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }
        public async Task<IPrincipal> HandleAsync(AccountCommand.Login command)
        {
            var authenticationCommand = new UserCommand.Authentication
            {
                AccountName = command.Name,
                Password = command.Password
            };
            var dto = await this._mediator.SendAsync<UserDto>(authenticationCommand);

            Assert.Build(dto)
                  .NotNull("Incorrect user name or password!");

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtClaimTypes.Subject, dto.Id.ToLower()));
            identity.AddClaim(new Claim(JwtClaimTypes.Name, dto.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, dto.Account.Name));
#warning 使用权限组进行替换
            identity.AddClaim(new Claim(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim,dto.AuthorizeCode??string.Empty, Constants.Claim.CustomType));
            identity.AddClaim(new Claim(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator, "1", Constants.Claim.CustomType));
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public Task<string> HandleAsync(AccountCommand.Register command)
        {
            return this._mediator.SendAsync<string>(command.To<UserCommand.Create>());
        }
    }
}
