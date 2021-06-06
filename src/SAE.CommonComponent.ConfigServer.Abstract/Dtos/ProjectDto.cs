using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SolutionId { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
