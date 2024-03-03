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
        public class Find
        {
            /// <summary>
            /// 集群标识
            /// </summary>
            /// <value></value>
            public string ClusterId { get; set; }
            /// <summary>
            /// 版本
            /// </summary>
            public string Version { get; set; }

            ///<inheritdoc/>
            public override string ToString()
            {
                return $"{Constants.Caching.ApplicationAuthorizeCommand_Find}_{this.ClusterId}_{this.Version}";
            }
        }
    }
}