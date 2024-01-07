using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    /// <summary>
    /// 规则
    /// </summary>
    public class RuleDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 左值
        /// </summary>
        /// <value></value>
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
        /// </remarks><value></value>
        public string Right { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}