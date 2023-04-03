using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Application.Commands
{
    /// <summary>
    /// 客户端认证凭据
    /// </summary>
    public class ClientCommand
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        public class Query : Paging
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
            /// 客户端标识
            /// </summary>
            /// <value></value>
            public string ClientId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            /// <value></value>

            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description{get;set;}
            /// <summary>
            /// 客户端端点
            /// </summary>
            /// <value></value>
            public ClientEndpointDto Endpoint { get; set; }
            /// <summary>
            /// 授权范围
            /// </summary>
            /// <value></value>
            public string[] Scopes { get; set; }
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
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description{get;set;}
            /// <summary>
            /// 客户端端点
            /// </summary>
            /// <value></value>
            public ClientEndpointDto Endpoint { get; set; }
            /// <summary>
            /// 授权范围
            /// </summary>
            /// <value></value>
            public string[] Scopes { get; set; }
        }
        /// <summary>
        /// 重新生成私钥
        /// </summary>
        public class RefreshSecret
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

    }
}