using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Events;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.BasicData.Domains
{
    /// <summary>
    /// 标签资源
    /// </summary>
    public class LabelResource : Document
    {
        /// <summary>
        /// ctor
        /// </summary>
        public LabelResource()
        {

        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="label"></param>
        /// <param name="command"></param>
        public LabelResource(Label label, LabelResourceCommand.Create command)
        {
            this.Apply<LabelResourceEvent.Create>(command, e =>
            {
                e.Id = $"{label.Id}_{command.ResourceType}_{command.ResourceId}".ToLower();
                e.LabelId = label.Id;
                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 标签标识
        /// </summary>
        /// <value></value>
        public string LabelId { get; set; }
        /// <summary>
        /// 资源标识
        /// </summary>
        /// <value></value>
        public string ResourceId { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        /// <value></value>
        public string ResourceType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        public DateTime CreateTime { get; set; }
    }
}