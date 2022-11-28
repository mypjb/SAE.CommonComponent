using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Authorize.Controllers
{
    /// <summary>
    /// 客户端认证凭据角色管理
    /// </summary>
    [ApiController]
    [Route("client/role")]
    public class ClientRoleController : Controller
    {
        private readonly IMediator _mediator;
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        public ClientRoleController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        /// <summary>
        /// 引用角色
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<IActionResult> Reference(ClientRoleCommand.ReferenceRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 删除角色引用
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete]
        public async Task<IActionResult> DeleteReference(ClientRoleCommand.DeleteRole command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 分页查询引用的角色
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<IPagedList<RoleDto>> Paging([FromQuery] ClientRoleCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<RoleDto>>(command);
        }
        /// <summary>
        /// 列出引用的角色
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<IEnumerable<RoleDto>> List([FromQuery] ClientRoleCommand.List command)
        {
            return await this._mediator.SendAsync<IEnumerable<RoleDto>>(command);
        }

    }
}
