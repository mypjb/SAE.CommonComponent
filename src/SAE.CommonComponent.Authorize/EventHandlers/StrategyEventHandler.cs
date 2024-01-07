using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Caching;
using SAE.CommonLibrary.Logging;

namespace SAE.CommonComponent.Authorize.EventHandlers
{
    /// <summary>
    /// 角色事件处理程序
    /// </summary>
    /// <inheritdoc/>
    public class StrategyEventHandler
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
        public StrategyEventHandler(IMediator mediator,
                                    ILogging<StrategyEventHandler> logging,
                                    IDistributedCache distributedCache)
        {
            this._mediator = mediator;
            this._logging = logging;
            this._distributedCache = distributedCache;
        }

    }
}