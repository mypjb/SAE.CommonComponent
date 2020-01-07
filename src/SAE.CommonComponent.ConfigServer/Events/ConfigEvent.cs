using SAE.CommonLibrary.EventStore;
using System;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ConfigCreateEvent : ConfigChangeEvent
    {
        /// <summary>
        /// 解决方案Id
        /// </summary>
        public string SolutionId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public string Id { get;  set; }
    }

    public class ConfigChangeEvent : IEvent
    {
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
       
    }
}
