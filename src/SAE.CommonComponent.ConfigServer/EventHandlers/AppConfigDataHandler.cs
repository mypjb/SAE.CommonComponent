using System.Threading.Tasks;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Caching;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.EventHandlers
{
    /// <summary>
    /// 角色事件处理程序
    /// </summary>
    /// <inheritdoc/>
    public class AppConfigDataHandler : IHandler<AppConfigCommand.Publish>
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
        public AppConfigDataHandler(IMediator mediator,
                                    ILogging<AppConfigDataHandler> logging,
                                    IDistributedCache distributedCache)
        {
            this._mediator = mediator;
            this._logging = logging;
            this._distributedCache = distributedCache;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task HandleAsync(AppConfigCommand.Publish command)
        {
            var json = command.ToJsonString();

            this._logging.Info($"应用配置变更:{json}");

            var appDto = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto>
            {
                Id = command.Id
            });

            if (appDto == null) return;

            this._logging.Info($"准备删除缓存应用配置缓存");
            await this._distributedCache.DeletePatternAsync($"{Constants.Caching.AppDataCommand_Find}{appDto.Id}");
            await this._distributedCache.DeletePatternAsync($"{Constants.Caching.AppDataCommand_Find}{appDto.ClusterId}");
        }
    }
}