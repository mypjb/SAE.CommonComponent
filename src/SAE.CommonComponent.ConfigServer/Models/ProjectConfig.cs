using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Models
{
    public class ProjectConfig : Document
    {
        public ProjectConfig()
        {

        }

        public ProjectConfig(Project project, Config config)
        {
            Assert.Build(project.SolutionId == config.SolutionId)
                  .True($"项目'{project.Name}',和配置'{config.Name}'所属解决方案不一致,无法建立引用");
            this.ProjectId = project.Id;
            this.ConfigId = config.Id;
        }

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectId { get; set; }
        public string ConfigId { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        protected override string GetIdentity()
        {
            return this.Id;
        }
    }
}
