using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Extension;
using Constants = SAE.CommonLibrary.Configuration.Constants;

namespace Microsoft.AspNetCore.Hosting
{
    public static class DefaultHostBuilderExtensions
    {
        /// <summary>
        /// Use Default Configure
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureDefault(this IHostBuilder builder, Action<SAEOptions> action = null)
        {
            if (action == null)
            {
                action = s =>
                {

                };
            }
#if DEBUG
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
#endif
            return builder
                          //.ConfigureJsonFileDirectorySource()
#if DEBUG
                          .ConfigureHostConfiguration(configure =>
                          {
                              configure.AddInMemoryCollection(new Dictionary<string, string>
                              {

                                {$"{Constants.Config.OptionKey}{Constants.ConfigSeparator}{nameof(SAEOptions.FileName)}","SAE.CommonComponent.Master" }
                              });
                          })
#endif
                          .ConfigureRemoteSource(action)
                          .UseAutofacProviderFactory();
        }
    }
}
