using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.Identity.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Identity.Controllers
{
    public class AppController : Controller
    {
        private readonly IMediator _mediator;
        public AppController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<object> Get(string id)
        {
            return await this._mediator.Send<AppDto>(id);
        }

        [HttpPost]
        public async Task<string> Add(AppCreateCommand command)
        {
            return await this._mediator.Send<string>(command);
        }

        [HttpPost]
        public async Task<object> Edit(AppChangeCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpPost("{action}/{id}")]
        public async Task<object> Refresh([FromRoute]AppRefreshSecretCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpPost("{action}")]
        public async Task<object> Status(AppChangeStatusCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpPost("{action}")]
        public async Task<object> ReferenceScope(AppReferenceScopeCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpPost("{action}")]
        public async Task<object> CancelReferenceScope(AppCancelReferenceScopeCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]AppQueryCommand command)
        {
            return await this._mediator.Send<IPagedList<AppDto>>(command);
        }

    }
}