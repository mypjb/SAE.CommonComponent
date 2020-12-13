using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Configuration;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class AppController : Controller
    {
        private readonly IMediator _mediator;

        public AppController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> Config([FromQuery] AppCommand.Config command)
        {
            var appConfig = await this._mediator.Send<AppConfigDto>(command);

            if (command.Version == appConfig.Version)
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

                query = query.Add(nameof(command.Version).ToLower(), appConfig.Version.ToString());


                var nextUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.Path}{query.ToUriComponent()}";

                this.HttpContext.Response.Headers.Add(SAEConfigurationProvider.ConfigUrl, nextUrl);

                return this.Json(appConfig.Data);
            }
        }
    }
}
