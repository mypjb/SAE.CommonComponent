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
    [Route("/cluster")]
    public class AppClusterController : Controller
    {
        private readonly IMediator _mediator;
        public AppClusterController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]Command.Find<AppClusterDto> command)
        {
            return await this._mediator.SendAsync<AppClusterDto>(command);
        }

        [HttpPost]
        public async Task<object> Add(AppClusterCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpPut]
        public async Task<object> Edit(AppClusterCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpPut("{action}")]
        public async Task<object> Status(AppClusterCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<AppCluster> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]AppClusterCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppClusterDto>>(command);
        }

    }
}