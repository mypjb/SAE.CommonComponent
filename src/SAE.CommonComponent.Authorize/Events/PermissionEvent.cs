using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Authorize.Events
{
    /// <summary>
    /// 权限事件
    /// </summary>
    public class PermissionEvent
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
            /// 系统标识
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public Status Status { get; set; }
        }
        /// <summary>
        /// 系统资源
        /// </summary>
        public class AppResource:IEvent
        {

            /// <summary>
            /// 系统资源标识
            /// </summary>
            /// <value></value>
            public string AppResourceId { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change : IEvent
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>

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
