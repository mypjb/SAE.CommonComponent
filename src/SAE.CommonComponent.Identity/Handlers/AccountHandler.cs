using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.VisualBasic;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Identity.Handlers
{
    public class AccountHandler : ICommandHandler<AccountCommand.Login, IPrincipal>,
                                  ICommandHandler<AccountCommand.Register, string>
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
                  .NotNull("用户名或密码错误!");

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtClaimTypes.Subject, dto.Id.ToLower()));
            identity.AddClaim(new Claim(JwtClaimTypes.Name, dto.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, dto.Account.Name));
            var authorizeCode = await this._mediator.SendAsync<AuthorizeCodeDto>(new UserRoleCommand.QueryUserAuthorizeCode
            {
                UserId = dto.Id
            });

            if (authorizeCode != null)
            {
                foreach (var kv in authorizeCode.Codes)
                {
                    identity.AddClaim(new Claim(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim,
                                                string.Format(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.GroupFormat,
                                                           kv.Key,
                                                           kv.Value),
                                                           Constants.Claim.CustomType));
                }

                foreach (var appid in authorizeCode.SuperAdmins)
                {
                    identity.AddClaim(new Claim(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator, appid,Constants.Claim.CustomType));
                }
            }

            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public Task<string> HandleAsync(AccountCommand.Register command)
        {
            return this._mediator.SendAsync<string>(command.To<UserCommand.Create>());
        }
    }
}
