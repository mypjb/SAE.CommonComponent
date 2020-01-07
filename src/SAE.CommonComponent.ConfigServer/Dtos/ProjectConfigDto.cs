using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class ProjectConfigDto:Dictionary<string,object>
    {
        public string ProjectId { get; set; }
    }
}
