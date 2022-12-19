using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Configuration;

namespace SAE.CommonComponent.Authorize.Controllers
{
     /// <summary>
    /// 超级管理员
    /// </summary>
    [Route("{controller}")]
    [ApiController]
    public class SuperAdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOptions<SAEOptions> _options;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="options"></param>
        public SuperAdminController(IMediator mediator, IOptions<SAEOptions> options)
        {
            this._mediator = mediator;
            this._options = options;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<IActionResult> Add(SuperAdminCommand.Create command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] SuperAdminCommand.Delete command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        
        /// <summary>
        /// 列出角色
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<object> List([FromQuery] SuperAdminCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<string>>(command);
        }
    }
}