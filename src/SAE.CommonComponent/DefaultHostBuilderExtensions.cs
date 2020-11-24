using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Microsoft.AspNetCore.Hosting
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
