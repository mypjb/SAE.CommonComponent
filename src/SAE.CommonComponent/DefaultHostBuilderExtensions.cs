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
        public const string ConfigServerName = "ConfigServer";
        public static IHostBuilder ConfigureDefault(this IHostBuilder builder)
        {
#if DEBUG
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
#endif
            return builder.ConfigureAppConfiguration((ctx, builder) =>
                          {
                              var configuration = ctx.Configuration.GetSection(ConfigServerName);
                              if (!ctx.HostingEnvironment.IsDevelopment() && configuration.Exists())
                              {
                                  var options = configuration.Get<SAEOptions>();
                                  Console.WriteLine($"采用远程配置:{options.ToJsonString()}");
                                  builder.AddRemoteSource(options);
                              }
                              else
                              {
                                  Console.WriteLine("采用本地配置文件");
                                  builder.AddJsonFileDirectory();
                              }
                          })
                          .UseAutofacProviderFactory();
        }
    }
}
