using System.Collections.Generic;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Application.Commands
{
    /// <summary>
    /// 系统资源
    /// </summary>
    public class AppResourceCommand
    {
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
            /// 资源访问地址
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// 资源访问谓词 (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 资源名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
            /// <summary>
            /// 资源访问地址
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// 资源访问谓词 (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 资源名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
            /// <summary>
            /// 资源访问地址
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// 资源访问谓词 (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }
        /// <summary>
        /// 列出系统所有资源
        /// </summary>
        public class List
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
        }
        /// <summary>
        /// 设置索引
        /// </summary>
        public class SetIndex
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 索引
            /// </summary>
            /// <value></value>
            public int Index { get; set; }
        }
    }
}