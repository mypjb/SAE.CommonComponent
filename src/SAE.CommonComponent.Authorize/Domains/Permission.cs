using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Domains
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public Permission()
        {

        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 系统标识
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 系统资源标识
        /// </summary>
        /// <value></value>
        public string AppResourceId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public PermissionCommand.AppResource Create(PermissionCommand.Create command)
        {
            this.Apply<PermissionEvent.Create>(command, @event =>
             {
                 @event.Id = Utils.GenerateId();
                 @event.CreateTime = DateTime.UtcNow;
                 @event.Status = Status.Enable;
             });

            var resourceCommand = new PermissionCommand.AppResource
            {
                Id = this.Id,
                AppResourceId = command.AppResourceId
            };

            return this.SetAppResource(resourceCommand) ? resourceCommand : null;
        }
        /// <summary>
        /// 设置资源
        /// </summary>
        /// <param name="command"></param>
        public bool SetAppResource(PermissionCommand.AppResource command)
        {
            if ((command.AppResourceId.IsNullOrWhiteSpace() && !this.AppResourceId.IsNullOrWhiteSpace()) ||
                (!command.AppResourceId.IsNullOrWhiteSpace() && this.AppResourceId != command.AppResourceId))
            {
                this.Apply<PermissionEvent.AppResource>(command);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 更改信息
        /// </summary>
        /// <param name="command"></param>
        public PermissionCommand.AppResource Change(PermissionCommand.Change command)
        {
            this.Apply<PermissionEvent.Change>(command);
            var resourceCommand = new PermissionCommand.AppResource
            {
                Id = this.Id,
                AppResourceId = command.AppResourceId
            };
            return this.SetAppResource(resourceCommand) ? resourceCommand : null;
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(PermissionCommand.ChangeStatus command) =>
            this.Apply<PermissionEvent.ChangeStatus>(command);
        /// <summary>
        /// 判断权限是否存在?如果存在则会触发异常。
        /// </summary>
        /// <exception cref="SAEException"/>
        /// <param name="provider">提供一个可以用来查询的委托</param>
        /// <returns></returns>
        public async Task NameExist(Func<Permission, Task<Permission>> provider)
        {
            var permission = await provider.Invoke(this);
            if (permission == null ||
                this.Id == permission.Id ||
                this.AppId != permission.AppId ||
                !this.Name.Equals(permission.Name, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(Permission)} name exist");
        }
    }
}
