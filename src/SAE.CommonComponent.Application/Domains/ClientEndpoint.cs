using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Abstract.Domains
{
    /// <summary>
    /// 第三方认证端点
    /// </summary>
    public class ClientEndpoint
    {
        /// <summary>
        /// 回调地址集合
        /// </summary>
        /// <value></value>
        public string[] RedirectUris { get; set; }
        /// <summary>
        /// 推出成功后的回调地址
        /// </summary>
        /// <value></value>
        public string[] PostLogoutRedirectUris { get; set; }
        /// <summary>
        /// 认证地址
        /// </summary>
        /// <value></value>
        public string SignIn { get; set; }
    }
}
