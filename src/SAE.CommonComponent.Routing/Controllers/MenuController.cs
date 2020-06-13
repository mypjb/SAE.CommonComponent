using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Domains;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;

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
            return await this._mediator.Send<string>(command);
        }
        [HttpDelete]
        public async Task<object> Remove([FromBody]BatchRemoveCommand<Menu> command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(MenuCommand.Change command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]MenuCommand.Find command)
        {
            return await this._mediator.Send<MenuDto>(command);
        }
        // [HttpGet("{action}")]
        // public async Task<object> Paging([FromQuery]MenuQueryCommand command)
        // {
        //     return await this._mediator.Send<IPagedList<MenuDto>>(command);
        // }
        [HttpGet("{action}")]
        public async Task<object> ALL()
        {
            return await this._mediator.Send<IEnumerable<MenuItemDto>>(new MenuCommand.List());
        }

    }
}
