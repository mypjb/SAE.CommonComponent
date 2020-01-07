using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Models
{
    public class Project : Document
    {
        public Project()
        {

        }

        public Project(ProjectCreateCommand command)
        {
            this.Apply<ProjectCreateEvent>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string SolutionId { get; set; }
        public int Version { get; set; }
        public DateTime CreateTime { get; set; }

        protected override string GetIdentity()
        {
            return this.Id;
        }


        public void Change(ProjectChangeCommand command) => this.Apply<ProjectChangeEvent>(command);

        public IEnumerable<ProjectConfig> Relevance(IEnumerable<Config> configs)
        {
            return configs.Select(config => new ProjectConfig(this, config)).ToArray();
        }
    }
}
