using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Routing.Commands
{
    /// <summary>
    /// 菜单命令
    /// </summary>
    public class MenuCommand
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 应用Id
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 父Id
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 访问路径
            /// </summary>
            /// <value></value>
            public string Path { get; set; }
            /// <summary>
            /// 菜单是否隐藏
            /// </summary>
            /// <value></value>

            public bool Hidden { get; set; }
        }

        /// <summary>
        /// 更改菜单属性
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
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
        }

        /// <summary>
        /// 获得菜单树
        /// </summary>
        public class Tree
        {
            public string AppId { get; set; }
        }
        /// <summary>
        /// 查找
        /// </summary>
        public class Finds
        {
            /// <summary>
            /// 菜单Id标识集合
            /// </summary>
            /// <value></value>
            public string[] Ids { get; set; }
        }
    }
}