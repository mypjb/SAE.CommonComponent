using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class ProjectConfigDto
    {
        public string Id { get; set; }
        public string ConfigId { get; set; }
        public string ProjectId { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
    }
}
