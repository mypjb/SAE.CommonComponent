using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Events;

namespace SAE.CommonComponent.Authorize.Domains
{
    public class Role : Document
    {
        public Role()
        {
            this.PermissionIds = Enumerable.Empty<string>().ToArray();
            this.MenuIds = Enumerable.Empty<string>().ToArray();
        }
        public Role(RoleCommand.Create command) : this()
        {
            this.Apply<RoleEvent.Create>(command, @event =>
             {
                 @event.Id = Utils.GenerateId();
                 @event.CreateTime = DateTime.UtcNow;
                 @event.Status = Status.Enable;
             });
        }
        public string Id { get; set; }


        /// <summary>
        /// role permission ids
        /// </summary>
        public string[] PermissionIds { get; set; }
        /// <summary>
        /// role menu ids
        /// </summary>
        public string[] MenuIds { get; set; }
        /// <summary>
        /// role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// role descriptor
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// role createtime
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// role status
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// role name is exist? exist trigger <seealso cref="SaeException"/><seealso cref="StatusCodes.ResourcesExist"/>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task NameExist(Func<string, Task<Role>> provider)
        {
            var role = await provider.Invoke(this.Name);
            if (role == null || this.Id.Equals(role.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(Role)} name exist");
        }
        /// <summary>
        /// change role base info
        /// </summary>
        /// <param name="command"></param>
        public void Change(RoleCommand.Change command) =>
            this.Apply<RoleEvent.Change>(command);
        /// <summary>
        /// change role status 
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(RoleCommand.ChangeStatus command) =>
            this.Apply<RoleEvent.ChangeStatus>(command);


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

        internal void ReferenceMenu(RoleCommand.ReferenceMenu command)
        {
            var menuIds = this.MenuIds.Concat(command.MenuIds)
                                                 .Distinct()
                                                 .ToArray();

            this.Apply(new RoleEvent.ReferenceMenu
            {
                MenuIds = menuIds
            });
        }

        internal void DeleteMenu(RoleCommand.DeleteMenu command)
        {
            if (!command?.MenuIds.Any() ?? false) return;

            var menuIds = this.MenuIds.ToList();

            menuIds.RemoveAll(s => command.MenuIds.Contains(s));

            this.Apply(new RoleEvent.ReferenceMenu
            {
                MenuIds = menuIds.ToArray()
            });
        }
    }
}
