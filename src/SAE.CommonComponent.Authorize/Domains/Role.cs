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

        }
        public Role(RoleCommand.Create command)
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
        public IEnumerable<string> PermissionIds { get; set; }

        /// <summary>
        /// role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// role descriptor
        /// </summary>
        public string Descriptor { get; set; }
        /// <summary>
        /// role createtime
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// role status
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// role name is exist? exist trigger <seealso cref="SaeException"/><seealso cref="StatusCode.ResourcesExist"/>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task NameExist(Func<string, Task<string>> provider)
        {
            var id = await provider.Invoke(this.Name);
            if (id.IsNullOrWhiteSpace() || this.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SaeException(StatusCode.ResourcesExist, $"{nameof(Role)} name exist");
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
        /// <summary>
        /// delete role
        /// </summary>
        public void Remove() => this.ChangeStatus(new RoleCommand.ChangeStatus { Status = Status.Delete });

        public void RelationPermission(RoleCommand.RelationPermission command)
        {
            var permissionIds= this.PermissionIds.Concat(command.PermissionIds)
                                                 .Distinct()
                                                 .ToArray();

            this.Apply(new RoleEvent.RelationPermission
            {
                PermissionIds = command.PermissionIds
            });
        }
    }
}
