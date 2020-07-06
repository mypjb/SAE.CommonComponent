using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class UserRoleController:Controller
    {
        private readonly IMediator _mediator;

        public UserRoleController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Reference(UserRoleCommand.Reference command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReference(UserRoleCommand.DeleteReference command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

    }
}
