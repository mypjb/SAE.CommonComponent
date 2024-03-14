﻿using System;
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
    [Route("{controller}")]
    [ApiController]
    public class RuleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOptions<SAEOptions> _options;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="options"></param>
        public RuleController(IMediator mediator, IOptions<SAEOptions> options)
        {
            this._mediator = mediator;
            this._options = options;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<object> Add(RuleCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Command.BatchDelete<Rule> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="command"></param>
        [HttpPut]
        public async Task<IActionResult> Edit(RuleCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<RuleDto> command)
        {
            return await this._mediator.SendAsync<RuleDto>(command);
        }
        /// <summary>
        /// 列出规则
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<object> List([FromQuery] RuleCommand.List command)
        {
            return await this._mediator.SendAsync<RuleCommand.List, IEnumerable<RuleDto>>(command);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] RuleCommand.Query command)
        {
            return await this._mediator.SendAsync<RuleCommand.Query, IPagedList<RuleDto>>(command);
        }
    }
}