using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.MultiTenant.Dtos
{
    /// <summary>
    /// 租户输出对象
    /// </summary>
    public class TenantDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        /// <value></value>
        public string ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 租户类型
        /// </summary>
        /// <value></value>
        public string Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        /// <value></value>
        public string Domain { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}