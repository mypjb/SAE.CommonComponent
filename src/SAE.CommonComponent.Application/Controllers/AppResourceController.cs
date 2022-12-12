using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Configuration;
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
        private readonly IOptions<SAEOptions> _options;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="options"></param>
        public AppResourceController(IMediator mediator, IOptions<SAEOptions> options)
        {
            this._mediator = mediator;
            this._options = options;
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
        /// <summary>
        /// 列出集群或系统的资源集合
        /// </summary>
        /// <param name="command"></param>
        [HttpGet("[action]")]
        public async Task<object> BitmapEndpoints([FromQuery] AppResourceCommand.BitmapEndpoints command)
        {
            var bitmapEndpointList = await this._mediator.SendAsync<BitmapEndpointListDto>(command);

            if (bitmapEndpointList.Version == command.Version)
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

            query = query.Add(nameof(command.Version).ToLower(), bitmapEndpointList.Version);

            var nextUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.Path}{query.ToUriComponent()}";

            this.HttpContext.Response.Headers.Add(this._options.Value.NextRequestHeaderName, nextUrl);

            return bitmapEndpointList.Data;
        }
    }
}