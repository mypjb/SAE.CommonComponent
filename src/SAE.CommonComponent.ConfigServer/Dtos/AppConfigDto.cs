using SAE.CommonLibrary.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class AppConfigDto
    {
        public AppConfigDto()
        {
            this.Data = new Dictionary<string, object>();
        }

        public int Version { get; set; }
        public IDictionary<string, object> Data { get; set; }

        public void Add(ProjectConfigDto projectConfig, ConfigDto config)
        {
            this.Data[projectConfig.Alias] = config?.Content.ToObject<object>();
        }
    }
}
