using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class EnvironmentVariableDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
