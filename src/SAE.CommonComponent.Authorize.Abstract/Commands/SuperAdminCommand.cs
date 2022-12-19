using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 超级管理员
    /// </summary>
    public class SuperAdminCommand
    {
        /// <summary>
        /// 创建客户端超管属性
        /// </summary>
        public class Create
        {

            /// <summary>
            /// 目标标识
            /// </summary>
            /// <value></value>
            public string TargetId { get; set; }
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
        }
        /// <summary>
        /// 删除客户端超管属性
        /// </summary>
        public class Delete
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
        }
        /// <summary>
        /// 返回超管列表
        ///</summary>
        public class List
        {
            /// <summary>
            /// 目标标识
            /// </summary>
            /// <value></value>
            public string TargetId { get; set; }
        }
    }
}