using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("app/config")]
    public class AppConfigController : Controller
    {
        private readonly IMediator _mediator;

        public AppConfigController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<object> Reference(AppConfigCommand.ReferenceConfig command)
        {
            await this._mediator.SendAsync(command);

            return this.Ok();
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<AppConfigDto> command)
        {
            return await this._mediator.SendAsync<AppConfigDto>(command);
        }



        [HttpPut]
        public async Task<object> Change(AppConfigCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete]
        public async Task<object> Delete(Command.BatchDelete<AppConfig> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpPost("{action}")]
        public async Task<object> Publish(AppConfigCommand.Publish command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{action}")]
        public async Task<object> Preview([FromQuery]AppConfigCommand.Preview command)
        {
            var data = await this._mediator.SendAsync<AppConfigDataPreviewDto>(command);
            return data;
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] AppConfigCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppConfigDto>>(command);
        }

        [HttpGet("reference/paging")]
        public async Task<object> ConfigPaging([FromQuery] AppConfigCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<ConfigDto>>(command);
        }
    }
}
