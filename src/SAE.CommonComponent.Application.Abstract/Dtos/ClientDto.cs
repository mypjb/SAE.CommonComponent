using System;
using System.Collections.Generic;
using SAE.CommonComponent.Application.Dtos;

namespace SAE.CommonComponent.Application.Dtos
{
    /// <summary>
    /// 客户端认证凭据
    /// </summary>
    public class ClientDto
    {
        /// <summary>
        /// 标识，也是唯一公钥
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 端点
        /// </summary>
        public ClientEndpointDto Endpoint { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        /// <value></value>
        public string Secret { get; set; }

        /// <summary>
        /// 授权域
        /// </summary>
        /// <value></value>
        public string[] Scopes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// client 状态
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
    }
}