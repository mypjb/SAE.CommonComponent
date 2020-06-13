using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class SolutionController:Controller
    {

        private readonly IMediator _mediator;

        public SolutionController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(SolutionCommand.Create command)
        {
            return await this._mediator.Send<string>(command);
        }
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute]RemoveCommand<Solution> command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(SolutionCommand.Change command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]SolutionCommand.Find command)
        {
            return await this._mediator.Send<SolutionDto>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]SolutionCommand.Query command)
        {
            return await this._mediator.Send<IPagedList<SolutionDto>>(command);
        }
    }
}
