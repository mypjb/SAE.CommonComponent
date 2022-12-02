using System;
using System.Collections.Generic;
using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Application.Events
{
    /// <summary>
    /// 集群事件
    /// </summary>
    public partial class AppClusterEvent
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
            /// <summary>
            /// 集群类型。（集群下的资源属于多租户时，应该设置该值，该值对应字典标识，一旦设置不可更改!)
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change : IEvent
        {
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        public class ChangeStatus : IEvent
        {
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }
    }
}