using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class AppController : Controller
    {
        private readonly IMediator _mediator;
        public AppController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]AppCommand.Find command)
        {
            return await this._mediator.Send<AppDto>(command);
        }

        [HttpPost]
        public async Task<object> Add(AppCommand.Create command)
        {
            return await this._mediator.Send<string>(command);
        }

        [HttpPut]
        public async Task<object> Edit(AppCommand.Change command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }
        [HttpPut("{action}/{id}")]
        public async Task<object> Refresh([FromRoute]AppCommand.RefreshSecret command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpPut("{action}")]
        public async Task<object> Status(AppCommand.ChangeStatus command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpPost("{action}")]
        public async Task<object> ReferenceScope(AppCommand.ReferenceScope command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpPost("{action}")]
        public async Task<object> CancelReferenceScope(AppCommand.CancelReferenceScope command)
        {
            await this._mediator.Send(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]AppCommand.Query command)
        {
            return await this._mediator.Send<IPagedList<AppDto>>(command);
        }

    }
}