using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class ProjectCommand
    {
        public class Publish
        {
            /// <summary>
            /// 项目Id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }

        public class Preview : Publish
        {
        }
        public class Create
        {
            public string Name { get; set; }
            public string SolutionId { get; set; }
            public int Version { get; set; }
            public string Id { get; set; }
        }

        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class RelevanceConfig
        {
            public string ProjectId { get; set; }
            public string[] ConfigIds { get; set; }
        }

        public class ConfigChangeAlias
        {
            public string Id { get; set; }
            public string Alias { get; set; }
        }

        public class Query : Paging
        {
            public string Name { get; set; }
            /// <summary>
            /// 解决方案Id
            /// </summary>
            public string SolutionId { get; set; }
        }

        public class ConfigQuery : Paging
        {
            /// <summary>
            /// 项目Id
            /// </summary>
            public string ProjectId { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }


    }
}
