using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Authorize.Controllers
{
    /// <summary>
    /// 规则管理
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class StrategyController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOptions<SAEOptions> _options;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="options"></param>
        public StrategyController(IMediator mediator, IOptions<SAEOptions> options)
        {
            this._mediator = mediator;
            this._options = options;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<object> Add(StrategyCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Command.BatchDelete<Strategy> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="command"></param>
        [HttpPut]
        public async Task<IActionResult> Edit(StrategyCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        [HttpPut("[action]")]
        public async Task<IActionResult> Status(StrategyCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="command"></param>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddRule([FromBody] StrategyCommand.AddRule command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<StrategyDto> command)
        {
            return await this._mediator.SendAsync<StrategyDto>(command);
        }
        /// <summary>
        /// 列出规则
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<object> List([FromQuery] StrategyCommand.List command)
        {
            return await this._mediator.SendAsync<StrategyCommand.List, IEnumerable<StrategyDto>>(command);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<object> Paging([FromQuery] StrategyCommand.Query command)
        {
            return await this._mediator.SendAsync<StrategyCommand.Query, IPagedList<StrategyDto>>(command);
        }


        /// <summary>
        /// 列出策略引用的规则
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<object> RuleList([FromQuery] StrategyCommand.RuleList command)
        {
            return await this._mediator.SendAsync<StrategyCommand.RuleList, IEnumerable<IEnumerable<RuleDto>>>(command);
        }

        /// <summary>
        /// 创建资源和策略关联对象
        /// </summary>
        /// <param name="command">操作命令</param>
        [HttpPost("[action]")]
        public async Task<object> Resource([FromBody] StrategyResourceCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        /// <summary>
        /// 创建资源和策略关联对象
        /// </summary>
        /// <param name="command">操作命令</param>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Resource([FromBody] Command.BatchDelete<Strategy> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        /// <summary>
        /// 创建资源和策略关联对象
        /// </summary>
        /// <param name="command">操作命令</param>
        [HttpGet("[action]")]
        public async Task<object> Resource([FromQuery] StrategyResourceCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<StrategyDto>>(command);
        }
    }
}
