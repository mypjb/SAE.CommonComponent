using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 客户端认证凭据角色
    /// </summary>
    public class ClientRoleDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 客户端认证凭据标识
        /// </summary>
        /// <value></value>
        public string ClientId { get; set; }
        /// <summary>
        /// 角色标识
        /// </summary>
        /// <value></value>
        public string RoleId { get; set; }
    }
}
