using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Controllers
{
    [ApiController]
    [Route("user/role")]
    public class UserRoleController:Controller
    {
        private readonly IMediator _mediator;

        public UserRoleController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Reference(UserRoleCommand.ReferenceRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReference(UserRoleCommand.DeleteRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<IPagedList<RoleDto>> Paging([FromQuery]UserRoleCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<RoleDto>>(command);
        }

    }
}
