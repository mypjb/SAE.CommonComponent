using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.BasicData.Commands
{
    /// <summary>
    /// 字典命令
    /// </summary>
    public class DictCommand
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 父级标识
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            /// <value></value>
            public int Sort { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change : Create
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
        }


        /// <summary>
        /// 查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 父级标识
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }

        }
        /// <summary>
        /// 列表
        /// </summary>
        public class List
        {
            /// <summary>
            /// 父级标识
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }

        }

        /// <summary>
        /// 查询树
        /// </summary>
        public class Tree
        {
            /// <summary>
            /// 父级标识
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// <para>类型,该类型为父级为顶级结构的字典</para>
            /// <para><see cref="ParentId"/>和Type只能应用一个，Type具有更高优先级</para>
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        public class Find
        {
            /// <summary>
            /// 使用带有层次结构的名称进行查找
            /// </summary>
            /// <remarks>
            ///  使用<c>/</c>进行分割
            /// </remarks>
            /// <value></value>
            public string Names { get; set; }
            ///<inheritdoc/>
            public override string ToString()
            {
                return $"{nameof(DictCommand)}{nameof(Find)}_{this.Names}";
            }
        }
    }
}