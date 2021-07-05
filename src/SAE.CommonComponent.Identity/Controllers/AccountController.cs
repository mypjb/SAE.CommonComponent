using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using StatusCodes = SAE.CommonLibrary.StatusCodes;

namespace SAE.CommonComponent.Identity.Controllers
{
    [Route("{controller}/{action}")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogging _logging;
        private readonly IHostEnvironment _environment;
        private readonly CookieAuthenticationOptions _cookieAuthenticationOptions;
        public AccountController(IMediator mediator,
                                 IIdentityServerInteractionService interaction,
                                 ILogging<AccountController> logging,
                                 IHostEnvironment environment,
                                IOptionsSnapshot<CookieAuthenticationOptions> optionsSnapshot)
        {
            this._mediator = mediator;
            this._interaction = interaction;
            this._logging = logging;
            this._environment = environment;
            this._cookieAuthenticationOptions = optionsSnapshot.Value;
        }
        [Route("~/home/error")]
        [AllowAnonymous]
        public async Task<object> Error(string errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);
            var messageJson = message.ToJsonString();
            if (messageJson.IsNullOrWhiteSpace())
            {
                throw new SAEException(StatusCodes.Unknown);
            }
            else
            {
                this._logging.Error(messageJson);
                throw new SAEException(StatusCodes.Custom, messageJson.Replace("\\\"", string.Empty));
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (this.Request.Query.ContainsKey(this._cookieAuthenticationOptions.ReturnUrlParameter))
            {
                var qs = this.Request.Query[this._cookieAuthenticationOptions.ReturnUrlParameter].ToString();

                var kv = this.NameCollection(qs.Substring(qs.IndexOf('?') + 1));

                string clientId;
                if (kv.TryGetValue("client_Id", out clientId))
                {
                    var app = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto> { Id = clientId });
                    var paramsters = app.Endpoint.SignIn.IndexOf('?') == -1 ? this.Request.QueryString.ToString() :
                                                                              this.Request.QueryString.ToString().Substring(1);
                    return this.Redirect($"{app.Endpoint.SignIn}{paramsters}");
                }
            }

            return this.StatusCode(404);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm]AccountCommand.Login command)
        {
            var principal = await this._mediator.SendAsync<IPrincipal>(command);

            var claimsPrincipal = new ClaimsPrincipal(principal);

            var properties = new AuthenticationProperties();

            properties.IsPersistent = command.Remember;

            await this.HttpContext.SignInAsync(
                claimsPrincipal,
                properties);

            var returnUrl = command.ReturnUrl.IsNullOrWhiteSpace() ? "/" :
                $"{this.Request.Scheme}://{this.Request.Host}{command.ReturnUrl}";
            return this.Redirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<string> Register([FromBody]AccountCommand.Register command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpGet, HttpPost]
        public IActionResult Logout()
        {
            return this.SignOut();
        }

        private IDictionary<string, string> NameCollection(string query)
        {
            var pairs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var index = item.IndexOf('=');

                if (index == -1 || item.Length - 1 == index)
                {
                    continue;
                }

                pairs.Add(item.Substring(0, index), item.Substring(index + 1));
            }

            return pairs;

        }

    }
}
