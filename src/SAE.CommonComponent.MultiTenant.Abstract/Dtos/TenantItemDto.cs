using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.MultiTenant.Dtos
{
    /// <summary>
    /// 树状结构的传输对象
    /// </summary>
    public class TenantItemDto : TenantDto
    {
        /// <summary>
        /// 子集
        /// </summary>
        /// <value></value>
        public IEnumerable<TenantItemDto> Items { get; set; }
    }
}