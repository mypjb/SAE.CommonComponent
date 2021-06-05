using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class TemplateController : Controller
    {
        private readonly IMediator _mediator;

        public TemplateController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(TemplateCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute]Command.Delete<Template> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(TemplateCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<TemplateDto> command)
        {
            return await this._mediator.SendAsync<TemplateDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]TemplateCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<TemplateDto>>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> List()
        {
            return await this._mediator.SendAsync<IEnumerable<TemplateDto>>(new Command.List<TemplateDto>());
        }
    }
}
