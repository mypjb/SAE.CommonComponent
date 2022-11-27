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
    /// 角色
    /// </summary>
    public class Role : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public Role()
        {
            this.PermissionIds = Enumerable.Empty<string>().ToArray();
        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public Role(RoleCommand.Create command) : this()
        {
            this.Apply<RoleEvent.Create>(command, @event =>
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
        /// 角色索引
        /// </summary>
        public string Index { get; set; }
        /// <summary>
        /// 权限码
        /// </summary>
        /// <value></value>
        public string PermissionCode { get; set; }
        /// <summary>
        /// 角色引用的权限集合
        /// </summary>
        public string[] PermissionIds { get; set; }
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
        /// 更改角色信息
        /// </summary>
        /// <param name="command"></param>
        public void Change(RoleCommand.Change command) =>
            this.Apply<RoleEvent.Change>(command);
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(RoleCommand.ChangeStatus command) =>
            this.Apply<RoleEvent.ChangeStatus>(command);
        /// <summary>
        /// 引用权限
        /// </summary>
        /// <param name="command"></param>

        public void ReferencePermission(RoleCommand.ReferencePermission command)
        {
            var permissionIds = this.PermissionIds.Concat(command.PermissionIds)
                                                  .Distinct()
                                                  .ToArray();

            this.Apply(new RoleEvent.ReferencePermission
            {
                PermissionIds = permissionIds
            });
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="command"></param>
        public void DeletePermission(RoleCommand.DeletePermission command)
        {

            if (!command?.PermissionIds.Any() ?? false) return;

            var permissionIds = this.PermissionIds.ToList();

            permissionIds.RemoveAll(s => command.PermissionIds.Contains(s));

            this.Apply(new RoleEvent.ReferencePermission
            {
                PermissionIds = permissionIds.ToArray()
            });
        }

        /// <summary>
        /// 角色是否存在，如果存在则触发异常。
        /// </summary>
        /// <exception cref="SAEException"/>
        /// <param name="provider">提供一个可以用来查询的委托</param>
        /// <returns></returns>
        public async Task NameExist(Func<Role, Task<Role>> provider)
        {
            var role = await provider.Invoke(this);
            if (role == null || this.Id.Equals(role.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(Role)} name exist");
        }
        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="command"></param>
        public void SetIndex(RoleCommand.SetIndex command)
        {
            this.Apply<RoleEvent.SetIndex>(command);
        }
        /// <summary>
        /// 更改权限码
        /// </summary>
        /// <param name="command"></param>
        public void ChangePermissionCode(RoleCommand.ChangePermissionCode command)
        {
            this.Apply<RoleEvent.ChangePermissionCode>(command);
        }
    }
}
