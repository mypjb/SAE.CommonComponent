using Microsoft.Extensions.Configuration;
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
        
        public static string Get(string key)
        {
            var configuration = ServiceFacade.GetService<IConfiguration>()
                                             .GetSection(Option);

            var value = configuration.GetValue<string>(key);

            return value;
        }
    }
}
