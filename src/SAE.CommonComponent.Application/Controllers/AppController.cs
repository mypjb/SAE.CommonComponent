using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
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
    [Route("{controller}")]
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
        [HttpPut("{action}/{id}")]
        public async Task<object> Refresh([FromRoute]AppCommand.RefreshSecret command)
        {
            var secret = await this._mediator.SendAsync<string>(command);
            return this.File(Encoding.ASCII.GetBytes(secret), "application/octet-stream",Constants.App.AppSecretFileName);
        }

        [HttpPut("{action}")]
        public async Task<object> Status(AppCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<AppDto> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]AppCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppDto>>(command);
        }


        [HttpPost("project")]
        public async Task<object> ReferenceProject([FromBody] AppCommand.ReferenceProject command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("project/{action}")]
        public async Task<IEnumerable<ProjectDetailDto>> Paging([FromQuery] AppCommand.ProjectQuery command)
        {
            return await this._mediator.SendAsync<IPagedList<ProjectDetailDto>>(command);
        }

    }
}