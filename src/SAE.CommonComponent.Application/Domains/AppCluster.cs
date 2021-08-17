using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Domains
{
    public class AppCluster:Document
    {
        public AppCluster()
        {

        }
        public AppCluster(AppClusterCommand.Create command)
        {
            this.Apply<AppClusterEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// app status
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }

        public void Change(AppClusterCommand.Change command)
        {
            this.Apply<AppClusterEvent.Change>(command);
        }

        public void ChangeStatus(AppClusterCommand.ChangeStatus command)
        {
            this.Apply<AppClusterEvent.ChangeStatus>(command);
        }

        public async Task NameExistAsync(Func<AppCluster, Task<AppCluster>> provider)
        {
            var cluster = await provider.Invoke(this);
            if (cluster == null)
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(AppCluster)} name exist");
        }
    }
}
