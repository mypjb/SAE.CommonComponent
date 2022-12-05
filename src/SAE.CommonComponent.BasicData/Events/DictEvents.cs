using System;
using System.Collections.Generic;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.BasicData.Events
{
    /// <summary>
    /// 字典事件
    /// </summary>
    public class DictEvent
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create : Change
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            /// <value></value>
            public DateTime CreateTime { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change : IEvent
        {
            /// <summary>
            /// 父级标识，为空则代表不存在父级
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            /// <value></value>
            public int Sort { get; set; }

        }

    }
}
