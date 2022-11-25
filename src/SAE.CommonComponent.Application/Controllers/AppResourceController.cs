using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Application.Controllers
{
    /// <summary>
    /// 系统资源
    /// </summary>
    [ApiController]
    [Route("/cluster/app/resource")]
    public class AppResourceController : Controller
    {
        private readonly IMediator _mediator;
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        public AppResourceController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<AppResourceDto> command)
        {
            return await this._mediator.SendAsync<AppResourceDto>(command);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<object> Add(AppResourceCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="command"></param>
        [HttpPut]
        public async Task<object> Edit(AppResourceCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<AppResourceDto> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<object> Paging([FromQuery] AppResourceCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppResourceDto>>(command);
        }
        /// <summary>
        /// 列出所有
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<object> List([FromQuery] AppResourceCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(command);
        }
    }
}