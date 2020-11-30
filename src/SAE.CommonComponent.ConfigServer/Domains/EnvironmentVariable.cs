using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    public class EnvironmentVariable:Document
    {
        public EnvironmentVariable()
        {

        }
        public EnvironmentVariable(EnvironmentVariableCommand.Create command)
        {
            this.Apply<EnvironmentVariableEvent.Create>(command,e=>
            {
                e.Id= Utils.GenerateId();
                e.CreateTime = DateTime.Now;
            });
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

        public void Change(EnvironmentVariableCommand.Change command)
        {
            this.Apply<EnvironmentVariableEvent.Change>(command);
        }
    }
}
