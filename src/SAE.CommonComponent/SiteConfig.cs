using SAE.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent
{
    public class SiteConfig : Dictionary<string, string>
    {
        public SiteConfig()
        {
        }

        public static string Get(string key)
        {
            var config = ServiceFacade.GetService<SiteConfig>();
            string value;
            if(config.TryGetValue(key,out value))
            {
                return value;
            }

            throw new SaeException(StatusCodes.ResourcesNotExist, $"'{nameof(SiteConfig)}' not exist '{key}' index");
        }
    }
}
