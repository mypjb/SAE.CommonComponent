using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.EventHandlers
{
    /// <summary>
    /// 角色事件处理程序
    /// </summary>
    /// <inheritdoc/>
    public class RoleEventHandler : IHandler<RoleCommand.Create>,
                                    IHandler<RoleCommand.PermissionChange>
    {
        private readonly IDocumentStore _documentStore;
        private readonly IMediator _mediator;
        private readonly ILogging _logging;
        private readonly IBitmapAuthorization _bitmapAuthorization;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="documentStore"></param>
        /// <param name="mediator"></param>
        /// <param name="logging"></param>
        /// <param name="bitmapAuthorization"></param>
        public RoleEventHandler(IDocumentStore documentStore,
                                IMediator mediator,
                                ILogging<RoleEventHandler> logging,
                                IBitmapAuthorization bitmapAuthorization)
        {
            this._documentStore = documentStore;
            this._mediator = mediator;
            this._logging = logging;
            this._bitmapAuthorization = bitmapAuthorization;
        }

        public async Task HandleAsync(RoleCommand.Create command)
        {
            this._logging.Info($"有新的角色'{command.Name}'被添加，准备设置其索引");

            var roleDtos = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new RoleCommand.List
            {
                AppId = command.AppId
            });

            var index = roleDtos.Max(s => s.Index);

            foreach (var role in roleDtos.OrderBy(s => s.CreateTime)
                                         .ThenBy(s => s.Id))
            {
                var message = $"角色(名称：{role.Name}，索引：{role.Index}，标识：{role.Id})";
                if (role.Index <= 0)
                {
                    this._logging.Info($"{message},索引尚未初始化。");
                }
                else if (roleDtos.Any(r => r.Id != role.Id && r.Index == role.Index))
                {
                    this._logging.Warn($"{message},存在相同的索引({role.Index})，重新进行分配。");
                }
                else
                {
                    continue;
                }

                var indexCommand = new RoleCommand.SetIndex
                {
                    Id = role.Id,
                    Index = ++index
                };

                this._logging.Info($"{message}->'{indexCommand.Index}',发送设置索引命令。");

                await this._mediator.SendAsync(indexCommand);
            }
            this._logging.Info($"角色'{command.Name}'添加完成。");
        }

        public async Task HandleAsync(RoleCommand.PermissionChange message)
        {
            var findCommand = new Command.Find<RoleDto>
            {
                Id = message.Id
            };

            var role = await this._mediator.SendAsync<RoleDto>(findCommand);

            var bits = new List<int>();

            if (role.Permissions.Any())
            {
                var appResourceIds = role.Permissions.Select(s => s.AppResourceId)
                                                     .ToArray();

                var appResourceListCommand = new AppResourceCommand.List
                {
                    AppId = role.AppId
                };

                var resourceDtos = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(appResourceListCommand);

                foreach (var resourceDto in resourceDtos)
                {
                    if (appResourceIds.Contains(resourceDto.Id))
                    {
                        bits.Add(resourceDto.Index);
                    }
                }
            }

            var permissionCode = this._bitmapAuthorization.GenerateCode(bits);

            var changePermissionCodeCommand = new RoleCommand.ChangePermissionCode
            {
                Id = role.Id,
                PermissionCode = permissionCode
            };

            await this._mediator.SendAsync(changePermissionCodeCommand);

        }
    }
}