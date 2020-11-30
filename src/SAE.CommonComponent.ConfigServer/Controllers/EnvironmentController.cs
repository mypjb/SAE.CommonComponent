using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("env")]
    public class EnvironmentController : Controller
    {
        private readonly IMediator _mediator;

        public EnvironmentController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<string> Add(EnvironmentVariableCommand.Create command)
        {
            return await this._mediator.Send<string>(command);
        }
        [HttpPut]
        public async Task<IActionResult> Put(EnvironmentVariableCommand.Change command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Command.Delete<EnvironmentVariable> command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpGet("{id}")]
        public async Task<object> Find([FromRoute] Command.Find<EnvironmentVariableDto> command)
        {
            return await this._mediator.Send<EnvironmentVariableDto>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> List()
        {
            return await this._mediator.Send<IEnumerable<EnvironmentVariable>>(new Command.List<EnvironmentVariable>());
        }
    }
}
