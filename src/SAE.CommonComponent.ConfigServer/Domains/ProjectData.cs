using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    public class ProjectData:Document
    {
        public ProjectData()
        {

        }
        public ProjectData(ProjectDataEvent.Create @event)
        {
            this.Apply<ProjectDataEvent.Create>(@event,e=>
            {
                e.Id = $"{@event.ProjectId}{Constants.DefaultSeparator}{@event.EnvironmentId}";
                e.Version = 1;
            });
        }

        public string Id { get; set; }
        /// <summary>
        /// 项目Id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 环境Id
        /// </summary>
        public string EnvironmentId { get; set; }
        /// <summary>
        /// 项目已发布的配置数据
        /// </summary>
        public string Data { get; set; }

        protected override string GetIdentity()
        {
            return this.Id;
        }

        public void Change(string data)
        {
            this.Apply(new ProjectDataEvent.Publish
            {
                Data = data,
                Version = this.Version + 1
            });
        }
    }
}
