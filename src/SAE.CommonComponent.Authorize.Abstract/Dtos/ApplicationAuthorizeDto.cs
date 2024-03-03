using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 规则
    /// </summary>
    public class ApplicationAuthorizeDto
    {
        /// <summary>
        /// 数据集
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

    }
}