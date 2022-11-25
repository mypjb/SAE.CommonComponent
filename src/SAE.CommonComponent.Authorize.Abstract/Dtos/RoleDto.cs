using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 角色
    /// </summary>
    public class RoleDto
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
        /// 角色引用的权限集合
        /// </summary>
        public string[] PermissionIds { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 权限集合
        /// </summary>
        /// <value></value>
        public PermissionDto[] Permissions { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        /// <value></value>
        public int Index { get; set; }
    }
}
