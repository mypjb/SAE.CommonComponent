using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Authorize.Controllers
{
    /// <summary>
    /// 用户角色管理
    /// </summary>
    [ApiController]
    [Route("user/role")]
    public class UserRoleController : Controller
    {
        private readonly IMediator _mediator;
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        public UserRoleController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        /// <summary>
        /// 引用角色
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<IActionResult> Reference(UserRoleCommand.ReferenceRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete]
        public async Task<IActionResult> DeleteReference(UserRoleCommand.DeleteRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 分页查询用户角色
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<IPagedList<RoleDto>> Paging([FromQuery] UserRoleCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<RoleDto>>(command);
        }

    }
}
