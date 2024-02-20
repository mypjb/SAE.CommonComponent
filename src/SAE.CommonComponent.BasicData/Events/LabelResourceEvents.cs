using System;
using System.Collections.Generic;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.BasicData.Events
{
    /// <summary>
    /// 字典事件
    /// </summary>
    public class LabelResourceEvent
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create : IEvent
        {
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
}
