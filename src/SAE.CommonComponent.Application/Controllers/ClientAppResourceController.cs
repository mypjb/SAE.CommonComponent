using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Controllers
{
    [ApiController]
    [Route("client/appresource")]
    public class ClientAppResourceController: Controller
    {
        private readonly IMediator _mediator;

        public ClientAppResourceController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Reference(ClientAppResourceCommand.ReferenceAppResource command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReference(ClientAppResourceCommand.DeleteAppResource command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<IPagedList<AppResourceDto>> Paging([FromQuery] ClientAppResourceCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppResourceDto>>(command);
        }
    }
}
