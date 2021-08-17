using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Controllers
{
    [ApiController]
    [Route("/cluster/app")]
    public class AppController : Controller
    {
        private readonly IMediator _mediator;
        public AppController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]Command.Find<AppDto> command)
        {
            return await this._mediator.SendAsync<AppDto>(command);
        }

        [HttpPost]
        public async Task<object> Add(AppCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpPut]
        public async Task<object> Edit(AppCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpPut("{action}")]
        public async Task<object> Status(AppCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<App> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]AppCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppDto>>(command);
        }
               
    }
}