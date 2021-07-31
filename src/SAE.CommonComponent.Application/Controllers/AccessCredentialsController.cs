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
    public class AccessCredentialsController : Controller
    {
        private readonly IMediator _mediator;
        public AccessCredentialsController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]Command.Find<AccessCredentialsDto> command)
        {
            return await this._mediator.SendAsync<AccessCredentialsDto>(command);
        }

        [HttpPost]
        public async Task<object> Add(AccessCredentialsCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpPut]
        public async Task<object> Edit(AccessCredentialsCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut("{action}/{id}")]
        public async Task<object> Refresh([FromRoute]AccessCredentialsCommand.RefreshSecret command)
        {
            var secret = await this._mediator.SendAsync<string>(command);
            return this.File(Encoding.ASCII.GetBytes(secret), "application/octet-stream",Constants.App.AppSecretFileName);
        }

        [HttpPut("{action}")]
        public async Task<object> Status(AccessCredentialsCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<AccessCredentialsDto> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]AccessCredentialsCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AccessCredentialsDto>>(command);
        }

    }
}