using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
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
        private readonly IDocumentStore _documentStore;
        private readonly IMediator _mediator;
        private readonly ILogging _logging;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="documentStore"></param>
        /// <param name="mediator"></param>
        /// <param name="logging"></param>
        public AppResourceEventHandler(IDocumentStore documentStore,
                                       IMediator mediator,
                                       ILogging<AppResourceEventHandler> logging)
        {
            this._documentStore = documentStore;
            this._mediator = mediator;
            this._logging = logging;
        }

        public async Task HandleAsync(AppResourceCommand.Create command)
        {
            this._logging.Info($"有新的系统资源'{command.Name}'被添加，准备设置其索引");

            var resources = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new AppResourceCommand.List
            {
                AppId = command.AppId
            });

            var index = resources.Max(s => s.Index);

            foreach (var resource in resources.OrderBy(s => s.CreateTime)
                                              .ThenBy(s => s.Id))
            {
                var message = $"系统资源(名称：{resource.Name}，索引：{resource.Index}，标识：{resource.Id})";
                if (resource.Index <= 0)
                {
                    this._logging.Info($"{message},索引尚未初始化。");
                }
                else if (resources.Any(r => r.Id != resource.Id && r.Index == resource.Index))
                {
                    this._logging.Warn($"{message},存在相同的索引({resource.Index})，重新进行分配。");
                }
                else
                {
                    continue;
                }

                var indexCommand = new AppResourceCommand.SetIndex
                {
                    Id = resource.Id,
                    Index = ++index
                };

                this._logging.Info($"{message}->'{indexCommand.Index}',发送设置索引命令。");

#if DEBUG
                await this.SendIndexAsync(indexCommand);

#endif
#if !DEBUG
                await this._mediator.SendAsync(indexCommand);
#endif

            }
            this._logging.Info($"系统资源'{command.Name}'添加完成。");
        }

        private async Task SendIndexAsync(AppResourceCommand.SetIndex command)
        {
            try
            {
                await this._mediator.SendAsync(command);
            }
            catch (Exception ex)
            {
                await this.SendIndexAsync(command);
            }

        }
    }
}