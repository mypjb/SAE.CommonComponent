using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class StrategyResourceDto
    {
        /// <summary>
        /// 资源标识
        /// </summary>
        /// <value></value>
        public string ResourceId { get; set; }
        /// <summary>
        /// 策略
        /// </summary>
        /// <value></value>
        public StrategyDto[] Strategies { get; set; }
    }
}