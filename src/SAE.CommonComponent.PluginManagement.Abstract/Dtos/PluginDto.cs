using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.PluginManagement.Dtos
{
    public class PluginDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
        public Status Status { get; set; }
        public string Entry { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
