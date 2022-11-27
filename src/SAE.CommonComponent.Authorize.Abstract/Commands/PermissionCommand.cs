﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 权限命令
    /// </summary>
    public class PermissionCommand
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
            /// <summary>
            /// 系统资源标识
            /// </summary>
            /// <value></value>
            public string AppResourceId { get; set; }
        }

        /// <summary>
        /// 更改基本信息
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
        /// 设置资源
        /// </summary>
        public class AppResource
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 系统资源标识
            /// </summary>
            /// <value></value>
            public string AppResourceId { get; set; }
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
        /// 批量查找
        /// </summary>
        public class Finds
        {
            /// <summary>
            /// 标识集合
            /// </summary>
            /// <value></value>
            public string[] Ids { get; set; }
        }
        /// <summary>
        /// 列出所有权限
        /// </summary>
        public class List
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
        }

    }


}
