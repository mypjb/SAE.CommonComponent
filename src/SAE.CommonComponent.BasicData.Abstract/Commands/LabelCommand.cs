using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.BasicData.Commands
{
    /// <summary>
    /// 标签命令
    /// </summary>
    public class LabelCommand
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
            /// 值
            /// </summary>
            /// <value></value>
            public string Value { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            /// <value></value>
            public string Creator { get; set; }
        }


        /// <summary>
        /// 查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            /// <value></value>
            public string Creator { get; set; }

        }
        /// <summary>
        /// 列表
        /// </summary>
        public class List
        {
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
        }
        /// <summary>
        /// 查找单个对象
        /// </summary>
        public class Find
        {
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 值
            /// </summary>
            /// <value></value>
            public string Value { get; set; }
        }
    }
}