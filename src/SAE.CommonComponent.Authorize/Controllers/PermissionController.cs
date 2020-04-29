using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.AspNetCore.Routing;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Controllers
{
    public class PermissionController:Controller
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(PermissionCommand.Create command)
        {
            return await this._mediator.Send<string>(command);
        }
        [HttpDelete]
        public async Task<object> Delete([FromBody]BatchRemoveCommand<Permission> command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpPut]
        public async Task<object> Put(PermissionCommand.Change command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpGet("{id}")]
        public async Task<object> Get(string id)
        {
            return await this._mediator.Send<PermissionDto>(id);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]PermissionCommand.Query command)
        {
            return await this._mediator.Send<IPagedList<PermissionDto>>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> ALL()
        {
            return await this._mediator.Send<IEnumerable<PermissionDto>>(new PermissionCommand.QueryALL());
        }

        [HttpGet("{action}")]
        public async Task<object> Location(IEnumerable<PermissionCommand.Create> commands)
        {
            return await this._mediator.Send<IEnumerable<BitmapEndpoint>>(commands);
        }
    }
}
