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
            var key = projectConfig.Alias;

            if (this.Data.ContainsKey(key))
            {

                key += "_";
                projectConfig.Alias = key;
                this.Add(projectConfig, config);
            }
            else
            {
                this.Data[key] = config?.Content.ToObject<object>();
            }

        }
    }
}
