using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
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
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogging _logging;

        public AccountController(IMediator mediator,
                                 IIdentityServerInteractionService interaction,
                                 ILogging<AccountController> logging)
        {
            this._mediator = mediator;
            this._interaction = interaction;
            this._logging = logging;
        }
        [Route("~/home/error")]
        [AllowAnonymous]
        public async Task<object> Error(string errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);
            var messageJson = message.ToJsonString();
            if (messageJson.IsNullOrWhiteSpace())
            {
                return new ErrorOutput(StatusCodes.Unknown);
            }
            else
            {
                this._logging.Error(messageJson);
                return new ErrorOutput(StatusCodes.Custom, messageJson);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return this.Redirect($"{Constants.DefaultMaster}{"/identity/login"}{this.Request.QueryString}");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<object> Login(AccountLoginCommand command)
        {
            var principal = await this._mediator.Send<IPrincipal>(command);

            var claimsPrincipal = new ClaimsPrincipal(principal);

            var properties = new AuthenticationProperties();

            properties.IsPersistent = command.Remember;


            await this.HttpContext.SignInAsync(claimsPrincipal, properties);

            return new
            {
                returnUrl = command.ReturnUrl.IsNullOrWhiteSpace() ? "/" :
                $"{this.Request.Scheme}://{this.Request.Host}{command.ReturnUrl}"
            };
        }

        [HttpGet, HttpPost]
        public IActionResult Logout()
        {
            return this.SignOut();
        }
    }
}
