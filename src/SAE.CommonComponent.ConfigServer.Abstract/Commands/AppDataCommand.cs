using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    /// <summary>
    /// 系统配置数据
    /// </summary>
    public partial class AppDataCommand
    {
        /// <summary>
        /// 查询
        /// </summary>
        public class Find
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 集群标识，（集群标识和系统标识只会应用一个，当同时存在时集群标识优先级更高）
            /// </summary>
            /// <value></value>
            public string ClusterId { get; set; }

            /// <summary>
            /// 环境类型
            /// </summary>
            public string Env { get; set; }
            /// <summary>
            /// 版本
            /// </summary>
            public int Version { get; set; }
            /// <summary>
            /// 私有标识
            /// </summary>
            public bool Private { get; set; }
            
            ///<inheritdoc/>
            public override string ToString()
            {
                var key = this.ClusterId.IsNullOrWhiteSpace() ? this.AppId : this.ClusterId;
                return $"{Constants.Caching.AppDataCommand_Find}{key}_{this.Env}_{this.Version}_{this.Private}";
            }
        }
    }
}
