using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    /// <summary>
    /// 资源授权命令
    /// </summary>
    public class ApplicationAuthorizeCommand
    {
        /// <summary>
        /// 查询资源权限列表
        /// </summary>
        public class List
        {
            /// <summary>
            /// 集群标识
            /// </summary>
            /// <value></value>
            public string ClusterId { get; set; }
        }
    }
}