using Microsoft.Extensions.Options;
using SAE.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent
{
    public class SiteConfig : Dictionary<string, string>
    {
        public const string Option = nameof(SiteConfig);
        public SiteConfig()
        {
        }

        public static string Get(string key)
        {
            var optionsSnapshot = ServiceFacade.GetService<IOptionsSnapshot<SiteConfig>>();
            var option = optionsSnapshot.Value;
            string value;
            if(option.TryGetValue(key,out value))
            {
                return value;
            }

            throw new SaeException(StatusCodes.ResourcesNotExist, $"'{nameof(SiteConfig)}' not exist '{key}' index");
        }
    }
}
