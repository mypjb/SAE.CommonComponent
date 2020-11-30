using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Microsoft.AspNetCore.Hosting
{ 
    public static class DefaultHostBuilderExtensions
    {
        public static IHostBuilder ConfigureDefault(this IHostBuilder builder)
        {
#if DEBUG
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
#endif
            return builder.ConfigureJsonFileDirectorySource()
                   .UseAutofacProviderFactory(); ;
        }
    }
}
