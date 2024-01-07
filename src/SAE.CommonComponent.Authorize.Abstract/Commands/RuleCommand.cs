using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 规则命令集合
    /// </summary>
    public class RuleCommand
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
            /// <summary>
            /// 左值
            /// </summary>
            public string Left { get; set; }
            /// <summary>
            /// 符号
            /// </summary>
            /// <remarks>
            /// ：<![CDATA[>、<、>=、<=]]>、=、!=、regex...
            /// </remarks>
            public string Symbol { get; set; }
            /// <summary>
            /// 右值
            /// </summary>
            /// <remarks>
            /// 再某些时候不存在右值，比如!$left
            /// </remarks>
            public string Right { get; set; }
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
        /// 查找多个
        /// </summary>
        public class Finds
        {
            /// <summary>
            /// 标识集合
            /// </summary>
            /// <value></value>
            public IEnumerable<string> Ids { get; set; }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 规则名称或描述关键字
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
        }
        /// <summary>
        /// 列出规则集合
        /// </summary>

        public class List
        {
            /// <summary>
            /// 规则名称或描述关键字
            /// </summary>
            public string Name { get; set; }
        }
    }
}