using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Authorization.ABAC;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 规则命令集合
    /// </summary>
    public class StrategyCommand
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 规则名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// 更改基本信息
        /// </summary>
        public class Change : Create
        {
            /// <summary>
            /// 标识
            /// </summary>
            public string Id { get; set; }
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        public class ChangeStatus
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        public class AddRule
        {
            /// <summary>
            /// 组合
            /// </summary>
            public class Combine
            {
                /// <summary>
                /// 规则标识
                /// </summary>
                public string Id { get; set; }
                /// <summary>
                /// 操作
                /// </summary>
                public LogicalOperator Operator { get; set; }
            }

            /// <summary>
            /// 策略标识
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 组合规则
            /// </summary>
            public Combine[] Combines { get; set; }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 规则名称或描述关键字
            ///</summary>
            public string Name { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public Status Status { get; set; }
        }

        /// <summary>
        /// 列出规则集合
        /// </summary>
        public class List
        {
            /// <summary>
            /// 规则名称或描述关键字
            ///</summary>
            public string Name { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public Status Status { get; set; }
        }

        /// <summary>
        /// 列出规则集合
        /// </summary>

        public class RuleList
        {
            /// <summary>
            /// 策略标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
        }

    }
}