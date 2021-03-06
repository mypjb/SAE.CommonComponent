﻿using System;
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



}
