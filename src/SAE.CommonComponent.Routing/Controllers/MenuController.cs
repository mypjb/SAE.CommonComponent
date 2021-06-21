using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Domains;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Routing.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class MenuController : Controller
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(MenuCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete]
        public async Task<object> Delete([FromBody] Command.BatchDelete<Menu> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(MenuCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<MenuDto> command)
        {
            return await this._mediator.SendAsync<MenuDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] MenuCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<MenuDto>>(command);
        }
        [HttpGet("{action}")]
        [AllowAnonymous]
        public async Task<object> Tree()
        {
            return await this._mediator.SendAsync<IEnumerable<MenuItemDto>>(new MenuCommand.Tree());
        }

        [HttpGet("permission/{action}")]
        public async Task<IEnumerable<PermissionDto>> Paging([FromQuery]MenuCommand.PermissionQuery command)
        {
            return await this._mediator.SendAsync<IPagedList<PermissionDto>>(command);
        }

        [HttpPost("permission")]
        public async Task<IActionResult> Relevance(MenuCommand.RelevancePermission command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("permission")]
        public async Task<IActionResult> DeletePermission(MenuCommand.DeletePermission command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
    }
}
