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
    [Route("{controller}/{action}")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Login(AccountLoginCommand command)
        {
            var principal = await this._mediator.Send<IPrincipal>(command);

            var claimsPrincipal = new ClaimsPrincipal(principal);

            var properties = new AuthenticationProperties();

            properties.IsPersistent = command.IsPersistent;


            await this.HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme,claimsPrincipal, properties);

            return this.Ok();
        }

        [HttpGet, HttpPost]
        public IActionResult Logout()
        {
            return this.SignOut();
        }
    }
}
