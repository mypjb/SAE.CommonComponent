using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Models
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config : Document
    {
        public Config()
        {

        }
        public Config(ConfigCreateCommand command)
        {
            this.Apply<ConfigCreateEvent>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 解决方案Id
        /// </summary>
        public string SolutionId { get; set; }
        /// <summary>
        /// 模板类型
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        public void Change(ConfigChangeCommand command)
        {
            this.Apply<ConfigChangeEvent>(command);
        }
    }
}
