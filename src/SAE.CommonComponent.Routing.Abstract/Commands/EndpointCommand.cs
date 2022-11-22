using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Routing.Abstract.Commands
{
    /// <summary>
    /// 端点命令
    /// </summary>
    public class EndpointCommand
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
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
            /// <summary>
            /// 路径
            /// </summary>
            /// <value></value>
            public string Path { get; set; }
            /// <summary>
            /// 请求谓词
            /// </summary>
            /// <value></value>
            public string Method { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            /// <value></value>
            public int Sort { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change
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
            /// <summary>
            /// 路径
            /// </summary>
            /// <value></value>
            public string Path { get; set; }
            /// <summary>
            /// 请求谓词
            /// </summary>
            /// <value></value>
            public string Method { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            /// <value></value>
            public int Sort { get; set; }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 路径
            /// </summary>
            /// <value></value>
            public string Path { get; set; }
            /// <summary>
            /// 请求谓词
            /// </summary>
            /// <value></value>
            public string Method { get; set; }
        }
    }
}