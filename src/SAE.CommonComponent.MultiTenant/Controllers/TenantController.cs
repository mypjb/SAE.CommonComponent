using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.MultiTenant.Commands;
using SAE.CommonComponent.MultiTenant.Domains;
using SAE.CommonComponent.MultiTenant.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.MultiTenant.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class TenantController : Controller
    {
        private readonly IMediator _mediator;

        public TenantController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(TenantCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<Tenant> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(TenantCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        [HttpPut("{action}")]
        public async Task<object> Status(TenantCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<TenantDto> command)
        {
            return await this._mediator.SendAsync<TenantDto>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] TenantCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<TenantDto>>(command);
        }
        [HttpGet("{action}")]
        public async Task<object> Tree([FromQuery] TenantCommand.Tree command)
        {
            return await this._mediator.SendAsync<IEnumerable<TenantItemDto>>(command);
        }

        [HttpGet("{action}")]
        public async Task<object> List([FromQuery] TenantCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<TenantDto>>(command);
        }
        [HttpPost("app")]
        public async Task<object> AppCreate(TenantCommand.App.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpPost("app/[action]")]
        public async Task<object> Paging(TenantCommand.App.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppDto>>(command);
        }
    }
}
