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
    [Route("{controller}")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(PermissionCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete]
        public async Task<object> Delete([FromBody] Command.BatchDelete<Permission> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(PermissionCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<PermissionDto> command)
        {
            return await this._mediator.SendAsync<PermissionDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] PermissionCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<PermissionDto>>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> List()
        {
            return await this._mediator.SendAsync<IEnumerable<PermissionDto>>(new Command.List<PermissionDto>());
        }

        [HttpGet, HttpPost]
        [Route("{action}")]
        public async Task<object> Location(IEnumerable<PermissionCommand.Create> commands)
        {
            return await this._mediator.SendAsync<IEnumerable<PermissionCommand.Create>, IEnumerable<BitmapEndpoint>>(commands);
        }
    }
}
