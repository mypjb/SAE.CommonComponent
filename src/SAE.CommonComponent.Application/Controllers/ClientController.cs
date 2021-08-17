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
    public class ClientController : Controller
    {
        private readonly IMediator _mediator;
        public ClientController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]Command.Find<ClientDto> command)
        {
            return await this._mediator.SendAsync<ClientDto>(command);
        }

        [HttpPost]
        public async Task<object> Add(ClientCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpPut]
        public async Task<object> Edit(ClientCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut("{action}/{id}")]
        public async Task<object> Refresh([FromRoute]ClientCommand.RefreshSecret command)
        {
            var secret = await this._mediator.SendAsync<string>(command);
            return this.File(Encoding.ASCII.GetBytes(secret), "application/octet-stream",Constants.App.AppSecretFileName);
        }

        [HttpPut("{action}")]
        public async Task<object> Status(ClientCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<ClientDto> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]ClientCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<ClientDto>>(command);
        }

    }
}