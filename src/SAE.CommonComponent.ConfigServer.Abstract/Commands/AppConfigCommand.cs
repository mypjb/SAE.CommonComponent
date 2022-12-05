using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class AppConfigCommand
    {
        /// <summary>
        /// 发布配置
        /// </summary>
        public class Publish
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }
        /// <summary>
        /// 预览
        /// </summary>

        public class Preview : Publish
        {
        }
        /// <summary>
        /// 引用配置
        /// </summary>
       
        public class ReferenceConfig
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 配置标识集
            /// </summary>
            /// <value></value>
            public string[] ConfigIds { get; set; }
        }
        /// <summary>
        /// 更改
        /// </summary>

        public class Change
        {
            /// <summary>
            /// 系统配置标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 别名
            /// </summary>
            /// <value></value>
            public string Alias { get; set; }
            /// <summary>
            /// private
            /// </summary>
            public bool Private { get; set; }
        }
        /// <summary>
        /// 分页查询
        /// </summary>

        public class Query : Paging
        {
            /// <summary>
            /// 系统标识
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }

    }
}
