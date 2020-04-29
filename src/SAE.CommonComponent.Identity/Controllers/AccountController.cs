using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        public async Task<IActionResult> Login(AccountLoginCommand command)
        {
            var principal = await this._mediator.Send<IPrincipal>(command);

            var claimsPrincipal = new ClaimsPrincipal(principal);

            var properties = new AuthenticationProperties();

            properties.IsPersistent = command.IsPersistent;

            return this.SignIn(claimsPrincipal,
                               properties,
                               IdentityServerConstants.DefaultCookieAuthenticationScheme);
        }


        public IActionResult Logout()
        {
            return this.SignOut();
        }
    }
}
