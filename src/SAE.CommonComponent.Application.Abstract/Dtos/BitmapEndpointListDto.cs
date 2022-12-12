using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Dtos
{
    /// <summary>
    /// 集群或系统位图端点列表
    /// </summary>
    public class BitmapEndpointListDto
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
