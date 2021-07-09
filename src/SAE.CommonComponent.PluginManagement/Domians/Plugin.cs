using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Events;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.PluginManagement.Domians
{
    public class Plugin : Document
    {
        public Plugin()
        {

        }

        public Plugin(PluginCommand.Create command)
        {
            this.Apply<PluginEvent.Create>(command,e=>
            {
                e.Id = e.Name.Trim().ToMd5().ToLower();
                e.CreateTime = DateTime.Now;
            });
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
        public Status Status { get; set; }
        public string Entry { get; set; }
        public DateTime CreateTime { get;set;}

        protected override string GetIdentity()
        {
            return this.Id;
        }

        public void Change(PluginCommand.Change command)
        {
            this.Apply<PluginEvent.Change>(command);
        }

        public void ChangeStatus(PluginCommand.ChangeStatus command)
        {
            this.Apply<PluginEvent.ChangeStatus>(command);
        }

        public void ChangeEntry(PluginCommand.ChangeEntry command)
        {
            this.Apply<PluginEvent.ChangeEntry>(command);
        }
    }
}
