using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 策略资源命令
    /// </summary>
    public class StrategyResourceCommand
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {

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
        }

        /// <summary>
        /// 显示资源和策略关联的列表
        /// </summary>
        /// <remarks>
        /// 资源所属类型<see cref="List.ResourceType"/>必填，如果填入<see cref="List.ResourceId"/>则只查询单个对象的所有策略
        /// </remarks> 
        public class List
        {
            /// <summary>
            /// 资源标识
            /// </summary>
            /// <value></value>
            public string ResourceId { get; set; }
            /// <summary>
            /// 资源类型(必填项)
            /// </summary>
            /// <value></value>
            public string ResourceType { get; set; }
        }

    }
}