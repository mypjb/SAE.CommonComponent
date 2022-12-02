using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Application.Controllers
{
    /// <summary>
    /// 集群管理
    /// </summary>
    [ApiController]
    [Route("/cluster")]
    public class AppClusterController : Controller
    {
        private readonly IMediator _mediator;
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        public AppClusterController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="command"></param>
        [HttpGet]
        public async Task<object> Get([FromQuery] AppClusterCommand.Find command)
        {
            return await this._mediator.SendAsync<AppClusterDto>(command);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command"></param>
        [HttpPost]
        public async Task<object> Add(AppClusterCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="command"></param>
        [HttpPut]
        public async Task<object> Edit(AppClusterCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        [HttpPut("{action}")]
        public async Task<object> Status(AppClusterCommand.ChangeStatus command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<AppCluster> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] AppClusterCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<AppClusterDto>>(command);
        }

    }
}