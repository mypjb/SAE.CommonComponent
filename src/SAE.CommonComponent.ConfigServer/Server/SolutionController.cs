using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Server
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
        public async Task<object> Add(SolutionCreateCommand command)
        {
            return await this._mediator.Send<string>(command);
        }
        [HttpDelete("{id}")]
        public async Task<object> Delete(RemoveCommand<Solution> command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpPut]
        public async Task<object> Put(SolutionChangeCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpGet("{id}")]
        public async Task<object> Get(GetByIdCommand<Solution> command)
        {
            return await this._mediator.Send<SolutionDto>(command);
        }
    }
}
