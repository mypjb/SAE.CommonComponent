using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
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
    public class RoleEventHandler : IHandler<RoleEvent.Create>,
                                    IHandler<RoleCommand.PermissionChange>,
                                    IHandler<RoleCommand.ChangeStatus>
    {
        private readonly IMediator _mediator;
        private readonly ILogging _logging;
        private readonly IBitmapAuthorization _bitmapAuthorization;
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logging"></param>
        /// <param name="bitmapAuthorization"></param>
        /// <param name="distributedCache"></param>
        public RoleEventHandler(IMediator mediator,
                                ILogging<RoleEventHandler> logging,
                                IBitmapAuthorization bitmapAuthorization,
                                IDistributedCache distributedCache)
        {
            this._mediator = mediator;
            this._logging = logging;
            this._bitmapAuthorization = bitmapAuthorization;
            this._distributedCache = distributedCache;
        }

        public async Task HandleAsync(RoleEvent.Create command)
        {
            this._logging.Info($"有新的角色'{command.Name}'被添加，准备设置其索引");

            var roleDtos = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new RoleCommand.List
            {
                AppId = command.AppId,
                Status = Status.ALL
            });

            if (!roleDtos.Any()) return;

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
            await this.RoleChangeCoreAsync(command.Id);
        }

        public async Task HandleAsync(RoleCommand.PermissionChange message)
        {
            var findCommand = new Command.Find<RoleDto>
            {
                Id = message.Id
            };

            var role = await this._mediator.SendAsync<RoleDto>(findCommand);

            var bits = new List<int>();

            var permissions = role.Permissions?.Any() ?? false ?
                              role.Permissions?.Where(s => s.Status == Status.Enable) :
                              Enumerable.Empty<PermissionDto>();

            if (permissions.Any())
            {
                var appResourceIds = permissions.Select(s => s.AppResourceId)
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
            await this.RoleChangeCoreAsync(message.Id);
        }

        public async Task HandleAsync(RoleCommand.ChangeStatus message)
        {
            await this.RoleChangeCoreAsync(message.Id);
        }

        private async Task RoleChangeCoreAsync(string roleId)
        {
            var findCommand = new Command.Find<RoleDto>
            {
                Id = roleId
            };

            var role = await this._mediator.SendAsync<RoleDto>(findCommand);

            Assert.Build(role)
                  .NotNull($"角色'{roleId}'不存在，或被删除！");

            var app = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto>
            {
                Id = role.AppId
            });

            await this._distributedCache.DeletePatternAsync($"^{Constants.Caching.Bitmap.BitmapDescriptors}{app.Id}");

            if (app == null)
            {
                this._logging.Error($"系统'{role.AppId}'不存在！");
            }
            else
            {
                await this._distributedCache.DeletePatternAsync($"^{Constants.Caching.Bitmap.BitmapDescriptors}{app.ClusterId}");
            }

        }
    }
}