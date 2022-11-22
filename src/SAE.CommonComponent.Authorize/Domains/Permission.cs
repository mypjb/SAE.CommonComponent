using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary;
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
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public Permission(PermissionCommand.Create command)
        {
            this.Apply<PermissionEvent.Create>(command, @event =>
             {
                 @event.Id = Utils.GenerateId();
                 @event.CreateTime = DateTime.UtcNow;
                 @event.Status = Status.Enable;
             });

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
        /// 更改信息
        /// </summary>
        /// <param name="command"></param>
        public void Change(PermissionCommand.Change command) =>
            this.Apply<PermissionEvent.Change>(command);
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
