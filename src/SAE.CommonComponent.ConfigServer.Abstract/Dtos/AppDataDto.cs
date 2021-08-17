using SAE.CommonLibrary.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class AppDataDto
    {
        public AppDataDto()
        {
            this.Data = new Dictionary<string, object>();
        }


        public int Version { get; set; }
        public IDictionary<string, object> Data { get; set; }

        public void Add(AppConfigDto appConfig, ConfigDto config)
        {
            var key = appConfig.Alias;

            if (this.Data.ContainsKey(key))
            {

                key += "_";
                this.Data[key] = config?.Content.ToObject<object>();
            }
            else
            {
                this.Data[key] = config?.Content.ToObject<object>();
            }

        }
    }
}
