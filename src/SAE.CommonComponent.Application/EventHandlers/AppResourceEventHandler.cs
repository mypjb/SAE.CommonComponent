using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Caching;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.EventHandlers
{
    /// <summary>
    /// 资源事件处理程序
    /// </summary>
    /// <inheritdoc/>
    public class AppResourceEventHandler : IHandler<AppResourceCommand.Create>
    {
        private readonly IMediator _mediator;
        private readonly ILogging _logging;
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logging"></param>
        /// <param name="distributedCache"></param>
        public AppResourceEventHandler(IMediator mediator,
                                       ILogging<AppResourceEventHandler> logging,
                                       IDistributedCache distributedCache)
        {
            this._mediator = mediator;
            this._logging = logging;
            this._distributedCache = distributedCache;
        }

        public async Task HandleAsync(AppResourceCommand.Create command)
        {
            this._logging.Info($"有新的系统资源'{command.Name}'被添加，准备设置其索引");
        }


    }
}