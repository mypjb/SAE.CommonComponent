using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 客户端认证凭据角色
    /// </summary>
    public class ClientRoleCommand
    {
        
        /// <summary>
        /// 引用角色
        /// </summary>
        public class ReferenceRole
        {
            /// <summary>
            /// 客户段标识
            /// </summary>
            /// <value></value>
            public string ClientId { get; set; }
            /// <summary>
            /// 角色标识集合
            /// </summary>
            /// <value></value>
            public string[] RoleIds { get; set; }
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        public class DeleteRole : ReferenceRole
        {
        }
        /// <summary>
        /// 查询客户端授权码
        /// </summary>
        public class QueryClientAuthorizeCode
        {
            public string ClientId { get; set; }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 客户端标识
            /// </summary>
            /// <value></value>
            public string ClientId { get; set; }
        }
        /// <summary>
        /// 列表查询
        /// </summary>
        public class List
        {
            /// <summary>
            /// 客户端标识
            /// </summary>
            /// <value></value>
            public string ClientId { get; set; }
        }
    }
}
