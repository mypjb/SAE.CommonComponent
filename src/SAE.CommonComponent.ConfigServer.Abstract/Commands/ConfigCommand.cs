using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class ConfigCommand
    {
        public class Create
        {
            /// <summary>
            /// 解决方案Id
            /// </summary>
            public string SolutionId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }
        public class Change
        {
            public string Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
        }

        public class Query : Paging
        {
            /// <summary>
            /// 解决方案Id
            /// </summary>
            public string SolutionId { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
            /// <summary>
            /// 配置名称
            /// </summary>
            public string Name { get; set; }
        }

    }
}
