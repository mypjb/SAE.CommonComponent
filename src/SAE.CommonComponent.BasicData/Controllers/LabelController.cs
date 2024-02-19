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
    [Route("[controller]")]
    public class LabelController : Controller
    {
        private readonly IMediator _mediator;

        public LabelController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(LabelCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<Label> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<LabelDto> command)
        {
            return await this._mediator.SendAsync<LabelDto>(command);
        }

        [HttpGet("[action]")]
        public async Task<object> Paging([FromQuery] LabelCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<LabelDto>>(command);
        }

        [HttpGet("[action]")]
        public async Task<object> List([FromQuery] LabelCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<LabelDto>>(command);
        }

    }
}
