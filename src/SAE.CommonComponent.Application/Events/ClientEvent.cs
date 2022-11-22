using System;
using System.Collections.Generic;
using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Application.Events
{
    /// <summary>
    /// 客户端认证凭据事件
    /// </summary>
    public partial class ClientEvent
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create : Change
        {
            /// <summary>
            /// 标识(公钥)
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 系统标识
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 私钥
            /// </summary>
            /// <value></value>
            public string Secret { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            /// <value></value>
            public DateTime CreateTime { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>
        public class Change : IEvent
        {
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
            /// 客户端端点
            /// </summary>
            /// <value></value>
            public ClientEndpoint Endpoint { get; set; }
            /// <summary>
            /// 区域标识
            /// </summary>
            /// <value></value>
            public string[] Scopes { get; set; }
        }
        /// <summary>
        /// 刷新私钥
        /// </summary>
        public class RefreshSecret : IEvent
        {
            /// <summary>
            /// 私钥
            /// </summary>
            /// <value></value>
            public string Secret { get; set; }
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        public class ChangeStatus : IEvent
        {
            /// <summary>
            /// 状态
            /// </summary>
            /// <value></value>
            public Status Status { get; set; }
        }
    }
}