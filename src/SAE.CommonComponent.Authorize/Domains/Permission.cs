using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Domains
{
    public class Permission : Document
    {
        public Permission()
        {

        }
        public Permission(PermissionCommand.Create command)
        {
            this.Apply<PermissionEvent.Create>(command, @event =>
             {
                 @event.Id = Utils.GenerateId();
                 @event.CreateTime = DateTime.UtcNow;
                 @event.Status = Status.Enable;
             });

        }
        public string Id { get; set; }
        /// <summary>
        /// permission name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  permission descriptor
        /// </summary>
        public string Descriptor { get; set; }
        /// <summary>
        /// permission flag
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// request method
        /// </summary>
        public AccessMethod Method { get; set; }

        /// <summary>
        /// permission createTime
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// permission name is exist? exist trigger <seealso cref="SaeException"/><seealso cref="StatusCodes.ResourcesExist"/>
        /// </summary>
        /// <param name="provider">role provider</param>
        /// <returns></returns>
        public async Task NameExist(Func<string, Task<Permission>> provider)
        {
            var permission = await provider.Invoke(this.Name);
            if (permission == null || this.Id.Equals(permission.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(Permission)} name exist");
        }
        /// <summary>
        /// permission status
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// change permission basic info
        /// </summary>
        /// <param name="command"></param>
        public void Change(PermissionCommand.Change command) =>
            this.Apply<PermissionEvent.Change>(command);
        /// <summary>
        /// change permission status 
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(PermissionCommand.ChangeStatus command) =>
            this.Apply<PermissionEvent.ChangeStatus>(command);

    }
}
