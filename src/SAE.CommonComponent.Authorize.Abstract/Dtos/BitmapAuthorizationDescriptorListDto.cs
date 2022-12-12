using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 集群或系统授权位图描述符集
    /// </summary>
    public class BitmapAuthorizationDescriptorListDto
    {
        /// <summary>
        /// 版本号
        /// </summary>
        /// <value></value>
        public string Version { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        /// <value></value>
        public object Data { get; set; }
    }
}