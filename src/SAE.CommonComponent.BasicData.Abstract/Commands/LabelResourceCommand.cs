using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.BasicData.Commands
{
    /// <summary>
    /// 标签资源
    /// </summary>
    public class LabelResourceCommand
    {
        /// <summary>
        /// 关联资源
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 创建人
            /// </summary>
            /// <value></value>
            public string Creator { get; set; }
            /// <summary>
            /// 标签名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 标签值
            /// </summary>
            /// <value></value>
            public string Value { get; set; }
            /// <summary>
            /// 资源标识
            /// </summary>
            /// <value></value>
            public string ResourceId { get; set; }
            /// <summary>
            /// 资源类型
            /// </summary>
            /// <value></value>
            public string ResourceType { get; set; }
        }

        /// <summary>
        /// 列出标签
        /// </summary>
        public class List
        {
            /// <summary>
            /// 资源标识，该字段需配合<seealso cref="ResourceType"/>使用，单独使用将不会其作用
            /// </summary>
            /// <value></value>
            public string ResourceId { get; set; }
            /// <summary>
            /// 资源类型，该字段需配合<seealso cref="ResourceId"/>使用，单独使用将不会其作用
            /// </summary>
            /// <value></value>
            public string ResourceType { get; set; }
        }
    }
}