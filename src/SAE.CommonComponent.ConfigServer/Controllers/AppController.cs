using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class AppController:Controller
    {
        private readonly IMediator _mediator;

        public AppController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpGet("{action}")]
        public async Task<IActionResult> Config([FromQuery]AppCommand.Config command)
        {
            var appConfig = await this._mediator.Send<AppConfigDto>(command);

            if (command.Version == appConfig.Version)
            {
                return this.StatusCode((int)HttpStatusCode.NotModified);
            }
            else
            {
                return this.Json(appConfig);
            }
        }
    }
}
