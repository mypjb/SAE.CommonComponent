using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Authorize.Events
{
    /// <summary>
    /// 角色事件
    /// </summary>
    public partial class RoleEvent
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
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            /// <value></value>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }
        /// <summary>
        /// 设置索引
        /// </summary>
        public class SetIndex : IEvent
        {
            /// <summary>
            /// 索引
            /// </summary>
            /// <value></value>
            public int Index { get; set; }
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
        /// <summary>
        /// 引用权限
        /// </summary>
        public class ReferencePermission : IEvent
        {
            /// <summary>
            /// 权限集合
            /// </summary>
            /// <value></value>
            public string[] PermissionIds { get; set; }
        }
        /// <summary>
        /// 更改权限码
        /// </summary>
        public class ChangePermissionCode : IEvent
        {
            /// <summary>
            /// 权限码
            /// </summary>
            /// <value></value>
            public string PermissionCode { get; set; }
        }
    }
}
