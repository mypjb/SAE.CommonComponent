using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Controllers
{
    [Route("{controller}")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(RoleCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Command.BatchDelete<Role> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Put(RoleCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpPut("{action}")]
        public async Task<IActionResult> Status(RoleCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<RoleDto> command)
        {
            return await this._mediator.SendAsync<RoleDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] RoleCommand.Query command)
        {
            return await this._mediator.SendAsync<RoleCommand.Query, IPagedList<RoleDto>>(command);
        }


        [HttpPost("permission")]
        public async Task<IActionResult> Relevance(RoleCommand.RelevancePermission command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("permission")]
        public async Task<IActionResult> DeletePermission(RoleCommand.DeletePermission command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("permission/{action}")]
        public async Task<IPagedList<PermissionDto>> Paging([FromQuery]RoleCommand.PermissionQuery command)
        {
            var paging = await this._mediator.SendAsync<IPagedList<PermissionDto>>(command);
            return paging;
        }

    }
}
