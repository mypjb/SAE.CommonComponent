using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Domains;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAE.CommonComponent.BasicData.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class DictController : Controller
    {
        private readonly IMediator _mediator;

        public DictController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(DictCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete]
        public async Task<object> Delete([FromBody] Command.BatchDelete<Dict> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(DictCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<DictDto> command)
        {
            return await this._mediator.SendAsync<DictDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] DictCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<DictDto>>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Tree([FromQuery]DictCommand.Tree command)
        {
            return await this._mediator.SendAsync<IEnumerable<DictItemDto>>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> List([FromQuery] DictCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<DictDto>>(command);
        }

    }
}
