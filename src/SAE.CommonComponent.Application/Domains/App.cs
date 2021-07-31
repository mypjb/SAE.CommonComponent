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
    public class App:Document
    {
        public App()
        {

        }
        public App(AppCommand.Create command)
        {
            this.Apply<AppEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// system id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// system name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// app config id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// app status
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }

        public void ReferenceProject(AppCommand.ReferenceProject command)
        {
            this.Apply<AppEvent.ReferenceProject>(command);
        }

        public void Change(AppCommand.Change command)
        {
            this.Apply<AppEvent.Change>(command);
        } 

        public void ChangeStatus(AppCommand.ChangeStatus command)
        {
            this.Apply<AppEvent.ChangeStatus>(command);
        }
    }
}
