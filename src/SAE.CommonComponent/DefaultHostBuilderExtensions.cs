using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Extension;

namespace Microsoft.AspNetCore.Hosting
{
    public static class DefaultHostBuilderExtensions
    {
        /// <summary>
        /// Use Default Configure
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureDefault(this IHostBuilder builder)
        {
#if DEBUG
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
#endif
            return builder
                          //.ConfigureJsonFileDirectorySource()
                          .ConfigureRemoteSource()
                          .UseAutofacProviderFactory();
        }
    }
}
