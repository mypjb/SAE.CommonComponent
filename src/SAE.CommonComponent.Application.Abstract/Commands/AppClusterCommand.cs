using System.Collections.Generic;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Application.Commands
{
    /// <summary>
    /// 集群命令
    /// </summary>
    public class AppClusterCommand
    {
        /// <summary>
        /// 查找单个对象，<see cref="Id"/>和<see cref="Type"/>
        /// 只会应用一个，<c>Id</c>优先级更高
        /// </summary>
        public class Find
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 集群名称
            /// </summary>
            /// <value></value>
            public string Key { get; set; }
            /// <summary>
            /// 集群类型。（集群下的资源属于多租户时，应该设置该值，该值对应字典标识，一旦设置不可更改!)
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 集群标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
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
            /// 集群类型。（集群下的资源属于多租户时，应该设置该值，该值对应字典标识，一旦设置不可更改!)
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
            /// <summary>
            /// 集群设置(当集群类型存在时，应该设置该对象)
            /// </summary>
            /// <value></value>
            public AppClusterSettingDto Setting { get; set; }
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
        /// 列出集群
        /// </summary>
        public class List
        {
            /// <summary>
            /// 集群类型。（集群下的资源属于多租户时，应该设置该值，该值对应字典标识，一旦设置不可更改!)
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
    }
}