using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Abstract.Commands;
using SAE.CommonComponent.Application.Abstract.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Controllers
{
    [ApiController]
    [Route("{controller}")]
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
            return await this._mediator.Send<IEnumerable<ScopeDto>>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> ALL()
        {
            return await this._mediator.Send<IEnumerable<ScopeDto>>(new ScopeQueryALLCommand());
        }

    }

}