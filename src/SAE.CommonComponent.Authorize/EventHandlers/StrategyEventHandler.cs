using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Caching;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.EventHandlers
{
    /// <summary>
    /// 角色事件处理程序
    /// </summary>
    /// <inheritdoc/>
    public class StrategyEventHandler : IHandler<StrategyCommand.ChangeStatus>,
                                        IHandler<StrategyCommand.AddRule>,
                                        IHandler<Command.BatchDelete<Strategy>>,
                                        IHandler<RuleCommand.Change>,
                                        IHandler<Command.BatchDelete<Rule>>
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

        public async Task HandleAsync(StrategyCommand.ChangeStatus command)
        {
            this._logging.Info("策略变更，清空授权缓存");
            await this.CacheClearCoreAsync();
        }

        public async Task HandleAsync(StrategyCommand.AddRule command)
        {
            this._logging.Info("添加新的策略规则，清空授权缓存");
            await this.CacheClearCoreAsync();
        }

        public async Task HandleAsync(Command.BatchDelete<Strategy> message)
        {
            this._logging.Info("策略删除");
            await this.CacheClearCoreAsync();
        }

        public async Task HandleAsync(RuleCommand.Change message)
        {
            this._logging.Info("规则变更");
            await this.CacheClearCoreAsync();
        }

        public async Task HandleAsync(Command.BatchDelete<Rule> message)
        {
            this._logging.Info("规则删除");
            await this.CacheClearCoreAsync();
        }

        private async Task CacheClearCoreAsync()
        {
            this._logging.Info("清理授权缓存");
            await this._distributedCache.DeletePatternAsync($"^{Constants.Caching.ApplicationAuthorizeCommand_Find}*");
        }
    }
}