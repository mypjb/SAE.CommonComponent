using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        /// <value></value>
        public string UserId { get; set; }
        /// <summary>
        /// 角色标识
        /// </summary>
        /// <value></value>
        public string RoleId { get; set; }
    }
}
