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
    public class AppResource:Document
    {
        public AppResource()
        {

        }

        public AppResource(AppResourceCommand.Create command)
        {
            this.Apply<AppResourceEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// identity
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// resource index relative to the app
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// resource name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// resource path
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// resource method (get、post、put...)
        /// </summary>
        public string Method { get; }

        public DateTime DateTime { get; set; }

        public void Change(AppResourceCommand.Change command)
        {
            this.Apply<AppResourceEvent.Change>(command);
        }
    }
}
