using System.Collections.Generic;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 角色命令
    /// </summary>
    public partial class RoleCommand
    {
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
            /// 名称
            /// </summary>
            /// <value></value>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
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

        /// <summary>
        /// 更改基本信息
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
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
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
        /// 更改权限码
        /// </summary>
        public class ChangePermissionCode
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 权限码
            /// </summary>
            /// <value></value>
            public string PermissionCode { get; set; }
        }
        /// <summary>
        /// 引用权限
        /// </summary>
        public class ReferencePermission
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 权限集合
            /// </summary>
            /// <value></value>
            public string[] PermissionIds { get; set; }
        }
        /// <summary>
        /// 移除权限引用
        /// </summary>
        public class DeletePermission : ReferencePermission
        {
        }

        /// <summary>
        /// 权限变更
        /// </summary>
        public class PermissionChange
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
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
        }
        /// <summary>
        /// 列出角色集合
        /// </summary>

        public class List
        {
            /// <summary>
            /// 权限标识
            /// </summary>
            /// <value></value>
            public string PermissionId { get; set; }
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }
        /// <summary>
        /// 查询集群或系统的授权位图集合
        /// </summary>
        public class BitmapAuthorizationDescriptors
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 集群标识
            /// </summary>
            /// <value></value>
            public string ClusterId { get; set; }
            /// <summary>
            /// 版本号
            /// </summary>
            /// <value></value>
            public string Version { get; set; }
            /// <summary>
            /// 返回缓存key
            /// </summary>
            public override string ToString()
            {
                return $"{Constants.Caching.Bitmap.BitmapDescriptors}{(this.ClusterId.IsNullOrWhiteSpace() ? this.AppId : this.ClusterId)}{Constants.DefaultSeparator}{Version}";
            }
        }
        /// <summary>
        /// 列出权限集合
        /// </summary>

        public class PermissionList
        {
            /// <summary>
            /// 角色标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
        }
    }
}
