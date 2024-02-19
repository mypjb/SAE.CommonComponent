using System;
using System.Collections.Generic;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.BasicData.Events
{
    /// <summary>
    /// 字典事件
    /// </summary>
    public class LabelEvent
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create:IEvent
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 值
            /// </summary>
            /// <value></value>
            public string Value { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            /// <value></value>
            public string Creator { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            /// <value></value>
            public DateTime CreateTime { get; set; }
        }
    }
}
