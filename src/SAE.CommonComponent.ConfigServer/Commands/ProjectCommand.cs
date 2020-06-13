using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class ProjectCommand
    {
        public class Find
        {
            public string Id { get; set; }
        }
        public class Create
        {
            public string Name { get; set; }
            public string SolutionId { get; set; }
            public int Version { get; set; }
        }

        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class RelevanceConfig
        {
            public string ProjectId { get; set; }
            public IEnumerable<string> ConfigIds { get; set; }
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
            public string ProjectId { get; set; }
        }

        public class VersionCumulation
        {
            public string ProjectId { get; set; }
        }
    }
}
