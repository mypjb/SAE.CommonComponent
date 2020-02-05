using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{

    public class ProjectCreateCommand
    {
        public string Name { get; set; }
        public string SolutionId { get; set; }
        public int Version { get; set; }
    }

    public class ProjectChangeCommand : ProjectCreateCommand
    {
        public string Id { get; set; }
    }

    public class ProjectRelevanceConfigCommand
    {
        public string Id { get; set; }
        public IEnumerable<string> ConfigIds { get; set; }
    }

    public class ProjectConfigChangeAliasCommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ProjectQueryCommand : Paging
    {
        /// <summary>
        /// 解决方案Id
        /// </summary>
        public string SolutionId { get; set; }
    }
}
