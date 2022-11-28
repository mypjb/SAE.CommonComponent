using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
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
    public class PermissionEventHandler : IHandler<PermissionCommand.AppResource>,
                                          IHandler<PermissionCommand.ChangeStatus>,
                                          ICommandHandler<Command.BatchDelete<Permission>>
    {
        private readonly IDocumentStore _documentStore;
        private readonly IMediator _mediator;
        private readonly IStorage _storage;
        private readonly ILogging _logging;
        private readonly IMessageQueue _messageQueue;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="documentStore"></param>
        /// <param name="mediator"></param>
        /// <param name="storage"></param>
        /// <param name="logging"></param>
        /// <param name="messageQueue"></param>
        public PermissionEventHandler(IDocumentStore documentStore,
                                      IMediator mediator,
                                      IStorage storage,
                                      ILogging<RoleEventHandler> logging,
                                      IMessageQueue messageQueue)
        {
            this._documentStore = documentStore;
            this._mediator = mediator;
            this._storage = storage;
            this._logging = logging;
            this._messageQueue = messageQueue;
        }

        public async Task HandleAsync(PermissionCommand.AppResource command)
        {
            await this.PermissionChangeHandleCoreAsync(command.Id);
        }

        public async Task HandleAsync(PermissionCommand.ChangeStatus command)
        {
            await this.PermissionChangeHandleCoreAsync(command.Id);
        }

        public async Task HandleAsync(Command.BatchDelete<Permission> command)
        {
            await command.Ids.ForEachAsync(this.PermissionChangeHandleCoreAsync);
        }

        private async Task PermissionChangeHandleCoreAsync(string permissionId)
        {
            var listCommand = new RoleCommand.List
            {
                PermissionId = permissionId
            };

            var roles = await this._mediator.SendAsync<IEnumerable<RoleDto>>(listCommand);

            foreach (var role in roles)
            {
                var permissionChangeCommand = new RoleCommand.PermissionChange
                {
                    Id = role.Id
                };
                await _messageQueue.PublishAsync(permissionChangeCommand);
            }
        }
    }
}