using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    public class Project : Document
    {
        public Project()
        {

        }

        public Project(ProjectCommand.Create command)
        {
            this.Apply<ProjectEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// 项目Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 解决方案id
        /// </summary>
        public string SolutionId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        protected override string GetIdentity()
        {
            return this.Id;
        }

        public void Change(ProjectCommand.Change command) => this.Apply<ProjectEvent.Change>(command);
   
        public IEnumerable<ProjectConfig> Relevance(IEnumerable<Config> configs)
        {
            return configs.Select(config => new ProjectConfig(this, config)).ToArray();
        }
    }

}
