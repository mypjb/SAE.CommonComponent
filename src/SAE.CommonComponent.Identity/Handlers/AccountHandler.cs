using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Dtos;
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
            var user = await this._mediator.SendAsync<UserDto>(authenticationCommand);

            Assert.Build(user).NotNull("用户名或密码错误!");

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtClaimTypes.Subject, user.Id.ToLower()));
            identity.AddClaim(new Claim(JwtClaimTypes.Name, user.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Account.Name));

            var rootDict = await this._mediator.SendAsync<DictDto>(new DictCommand.Find
            {
                Names = Constants.Dict.LabelUser
            });

            if (rootDict != null)
            {
                var labels = await this._mediator.SendAsync<IEnumerable<LabelDto>>(new LabelResourceCommand.List
                {
                    ResourceId = user.Id,
                    ResourceType = rootDict.Id
                });

                foreach (var label in labels)
                {
                    if (!identity.Claims.Any(s => s.Type.Equals(label.Name) &&
                                            s.Value.Equals(label.Value)))
                    {
                        identity.AddClaim(new Claim($"{Constants.Authorize.CustomPrefix}{label.Name}", label.Value, Constants.Claim.CustomType));
                    }
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
