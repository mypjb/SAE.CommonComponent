using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SAE.CommonComponent
{
    public static class DefaultHostBuilderExtensions
    {
        public static IHostBuilder ConfigureDefault(this IHostBuilder builder)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            
            return builder.ConfigureJsonFileDirectorySource()
                   .UseAutofacProviderFactory(); ;
        }
    }
}
