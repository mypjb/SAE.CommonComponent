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
    /// 角色管理
    /// </summary>
    [Route("{controller}")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOptions<SAEOptions> _options;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="options"></param>
        public RoleController(IMediator mediator, IOptions<SAEOptions> options)
        {
            this._mediator = mediator;
            this._options = options;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<object> Add(RoleCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Command.BatchDelete<Role> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="command"></param>
        [HttpPut]
        public async Task<IActionResult> Edit(RoleCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        [HttpPut("{action}")]
        public async Task<IActionResult> Status(RoleCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<RoleDto> command)
        {
            return await this._mediator.SendAsync<RoleDto>(command);
        }
        /// <summary>
        /// 列出角色
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<object> List([FromQuery] RoleCommand.List command)
        {
            return await this._mediator.SendAsync<RoleCommand.List, IEnumerable<RoleDto>>(command);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] RoleCommand.Query command)
        {
            return await this._mediator.SendAsync<RoleCommand.Query, IPagedList<RoleDto>>(command);
        }

        /// <summary>
        /// 引用权限
        /// </summary>
        /// <param name="command"></param>
        [HttpPost("permission")]
        public async Task<IActionResult> ReferencePermission(RoleCommand.ReferencePermission command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete("permission")]
        public async Task<IActionResult> DeletePermission(RoleCommand.DeletePermission command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 分页查询角色引用的权限
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("permission/{action}")]
        public async Task<IEnumerable<PermissionDto>> Paging([FromQuery] RoleCommand.PermissionList command)
        {
            return await this._mediator.SendAsync<IEnumerable<PermissionDto>>(command);
        }

        [HttpGet("~/.bitmaps")]
        public async Task<object> ClusterAppList([FromQuery] RoleCommand.BitmapAuthorizationDescriptors command)
        {
            var roleClusterAppList = await this._mediator.SendAsync<BitmapAuthorizationDescriptorListDto>(command);

            if (roleClusterAppList.Version == command.Version)
            {
                return this.StatusCode((int)HttpStatusCode.NotModified);
            }

            var query = new QueryString();
            foreach (var kv in this.Request.Query)
            {
                if (!kv.Key.Equals(nameof(command.Version), StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Add(kv.Key, kv.Value);
                }
            }

            query = query.Add(nameof(command.Version).ToLower(), roleClusterAppList.Version);

            var nextUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.Path}{query.ToUriComponent()}";

            this.HttpContext.Response.Headers.Add(this._options.Value.NextRequestHeaderName, nextUrl);

            return roleClusterAppList.Data;
        }

    }
}
