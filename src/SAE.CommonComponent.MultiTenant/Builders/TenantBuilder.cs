using SAE.CommonComponent.MultiTenant.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.MultiTenant
{
    /// <summary>
    /// 构建者
    /// </summary>
    /// <inheritdoc/>
    public class TenantBuilder : IBuilder<IEnumerable<TenantDto>>
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public TenantBuilder()
        {
            
        }

        public async Task Build(IEnumerable<TenantDto> model)
        {

        }
    }
}
