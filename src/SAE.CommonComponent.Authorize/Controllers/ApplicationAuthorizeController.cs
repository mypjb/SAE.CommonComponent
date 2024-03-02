using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary.Abstract.Authorization.ABAC;
using SAE.CommonLibrary.Abstract.Mediator;

namespace SAE.CommonComponent.Authorize.Controllers
{
    /// <summary>
    /// 规则管理
    /// </summary>
    [Route("app/resource/authorize")]
    [ApiController]
    public class ApplicationAuthorizeController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationAuthorizeController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<object> List([FromQuery]ApplicationAuthorizeCommand.List command)
        {
            return await this._mediator.SendAsync<object>(command);
        }
    }
}
