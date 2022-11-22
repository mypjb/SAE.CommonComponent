using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRoleCommand
    {
        /// <summary>
        /// 引用角色
        /// </summary>
        public class ReferenceRole
        {
            /// <summary>
            /// 用户标识
            /// </summary>
            /// <value></value>
            public string UserId { get; set; }
            /// <summary>
            /// 角色标识集合
            /// </summary>
            /// <value></value>
            public string[] RoleIds { get; set; }
        }
        /// <summary>
        /// 删除角色引用
        /// </summary>
        public class DeleteRole : ReferenceRole
        {
        }
        /// <summary>
        /// 查询用户授权标识
        /// </summary>
        public class QueryUserAuthorizeCode
        {
            /// <summary>
            /// 用户标识
            /// </summary>
            /// <value></value>
            public string UserId { get; set; }
        }
        /// <summary>
        /// 分页查询引用的角色
        /// </summary>
        public class Query:Paging
        {
            /// <summary>
            /// 用户标识
            /// </summary>
            /// <value></value>
            public string UserId { get; set; }
            /// <summary>
            /// 是否引用
            /// </summary>
            /// <value></value>
            public bool Referenced { get; set; }
        }
    }
}
