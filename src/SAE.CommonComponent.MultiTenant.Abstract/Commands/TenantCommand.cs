using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonComponent;

namespace SAE.CommonComponent.MultiTenant.Commands
{
    /// <summary>
    /// 租户
    /// </summary>
    public class TenantCommand
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create
        {
            /// <summary>
            /// 父级
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 租户类型
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
            /// <summary>
            /// 域名
            /// </summary>
            /// <value></value>
            public string Domain { get; set; }
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
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 租户类型
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
            /// <summary>
            /// 域名
            /// </summary>
            /// <value></value>
            public string Domain { get; set; }
        }

        /// <summary>
        /// 查询
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 关键字（对域名、名称、描述进行模糊匹配）
            /// </summary>
            /// <value></value>
            public string Key { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
        /// <summary>
        /// 列出所有租户信息
        /// </summary>
        public class List
        {
            /// <summary>
            /// 父级
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
        /// <summary>
        /// 返回树列表
        /// </summary>
        public class Tree
        {
            /// <summary>
            /// 父级
            /// </summary>
            /// <value></value>
            public string ParentId { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            /// <value></value>
            public string Type { get; set; }
        }
        /// <summary>
        /// 租户的系统
        /// </summary>
        public class App
        {
            /// <summary>
            /// 创建
            /// </summary>
            public class Create
            {
                /// <summary>
                /// 租户Id
                /// </summary>
                /// <value></value>
                public string TenantId { get; set; }
                /// <summary>
                /// 应用类型
                /// </summary>
                /// <value></value>
                public string Type { get; set; }
                /// <summary>
                /// 名称
                /// </summary>
                /// <value></value>
                public string Name { get; set; }
                /// <summary>
                /// 域名
                /// </summary>
                /// <value></value>
                public string Domain { get; set; }
                /// <summary>
                /// 描述
                /// </summary>
                /// <value></value>
                public string Description { get; set; }
            }
            /// <summary>
            /// 分页查询
            /// </summary>
            public class Query : Paging
            {
                /// <summary>
                /// 租户标识(必填)
                /// </summary>
                /// <value></value>
                public string TenantId { get; set; }
                /// <summary>
                /// 系统类型
                /// </summary>
                /// <value></value>
                public string Type { get; set; }
                /// <summary>
                /// 关键字
                /// </summary>
                /// <value></value>
                public string Key { get; set; }
            }
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
    }
}