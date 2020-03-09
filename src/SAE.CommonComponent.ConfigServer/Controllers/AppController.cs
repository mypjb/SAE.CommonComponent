using System;
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
        public async Task<object> Config(AppConfigCommand command)
        {
            var appConfig = await this._mediator.Send<AppConfigDto>(command);
            return command.Version == appConfig.Version ? new { appConfig.Version } : (object)appConfig;
        }
    }
}
