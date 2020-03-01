using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using System.Collections.Generic;

namespace SAE.CommonComponent.Identity.Controllers
{
    public class ScopeController : Controller
    {
        private readonly IMediator _mediator;
        public ScopeController(IMediator mediator)
        {
            this._mediator = mediator;

        }
        [HttpPost]
        public async Task<object> Add(ScopeCreateCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpDelete]
        public async Task<object> Remove(ScopeRemoveCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]ScopeQueryCommand command)
        {
            return await this._mediator.Send<IEnumerable<string>>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> List(ScopeQueryALLCommand command)
        {
            return await this._mediator.Send<IEnumerable<string>>(command);
        }

    }

}