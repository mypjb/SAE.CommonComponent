﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("client/role")]
    public class ClientRoleController : Controller
    {
        private readonly IMediator _mediator;

        public ClientRoleController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Reference(ClientRoleCommand.ReferenceRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReference(ClientRoleCommand.DeleteRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<IPagedList<RoleDto>> Paging([FromQuery] ClientRoleCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<RoleDto>>(command);
        }

    }
}
