using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Domians;
using SAE.CommonComponent.PluginManagement.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAE.CommonComponent.PluginManagement.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class PluginController : Controller
    {
        private readonly IMediator _mediator;

        public PluginController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        
        [HttpDelete]
        public async Task<object> Delete([FromBody] Command.BatchDelete<Plugin> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(PluginCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut("{action}")]
        public async Task<object> Status(PluginCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut("{action}")]
        public async Task<object> Entry(PluginCommand.ChangeEntry command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<PluginDto> command)
        {
            return await this._mediator.SendAsync<PluginDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] PluginCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<PluginDto>>(command);
        }
       
    }
}
