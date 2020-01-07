using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Test;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit.Abstractions;

namespace SAE.CommonComponent.ConfigServer.Test
{
    public abstract class ControllerTest : BaseTest
    {
        public ControllerTest(ITestOutputHelper output) : base(output)
        {

        }


        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }
    }
}
