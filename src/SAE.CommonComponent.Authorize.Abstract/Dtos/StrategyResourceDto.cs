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
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 资源类型，和字典相互关联
        /// </summary>
        /// <value></value>
        public string ResourceType { get; set; }
        /// <summary>
        /// 资源标识
        /// </summary>
        /// <value></value>
        public string ResourceId { get; set; }
        /// <summary>
        /// 策略标识
        /// </summary>
        /// <value></value>
        public string StrategyId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 策略
        /// </summary>
        /// <value></value>
        public StrategyDto Strategy { get; set; }
    }
}