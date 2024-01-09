using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Authorize.Events
{
    /// <summary>
    /// 策略事件
    /// </summary>
    public partial class StrategyEvent
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create : Change
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            /// <value></value>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }

        /// <summary>
        /// 更改
        /// </summary>
        public class Change : IEvent
        {
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }

        }
        /// <summary>
        /// 更改状态
        /// </summary>
        public class ChangeStatus : IEvent
        {
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        public class AddRule : IEvent
        {
            /// <summary>
            /// 规则组合
            /// </summary>
            public RuleCombine RuleCombine { get; set; }
        }
        /// <summary>
        /// 构建事件
        /// </summary>
        public class Build : IEvent
        {
            /// <summary>
            /// 表达式
            /// </summary>
            /// <value></value>
            public string Expression { get; set; }
        }

    }
}