using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Authorization.ABAC;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Controllers
{
    /// <summary>
    /// 规则管理
    /// </summary>
    [Route("app/resource/authorize")]
    [ApiController]
    public class ApplicationAuthorizeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOptions<SAEOptions> _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="options"></param>
        public ApplicationAuthorizeController(IMediator mediator, IOptions<SAEOptions> options)
        {
            this._mediator = mediator;
            this._options = options;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> List([FromQuery] ApplicationAuthorizeCommand.Find command)
        {
            var applicationAuthorizeDto = await this._mediator.SendAsync<ApplicationAuthorizeDto>(command);

            if (command.Version == applicationAuthorizeDto.Version)
            {
                return this.StatusCode((int)HttpStatusCode.NotModified);
            }
            else
            {
                var query = new QueryString();
                foreach (var kv in this.Request.Query)
                {
                    if (!kv.Key.Equals(nameof(command.Version), StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Add(kv.Key, kv.Value);
                    }
                }

                query = query.Add(nameof(command.Version).ToLower(), applicationAuthorizeDto.Version.ToString());

                var nextUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.Path}{query.ToUriComponent()}";

                this.HttpContext.Response.Headers.Append(this._options.Value.NextRequestHeaderName, nextUrl);

                return this.Content(applicationAuthorizeDto.Data.ToJsonString(), MediaTypeNames.Application.Json);
            }
        }
    }
}
