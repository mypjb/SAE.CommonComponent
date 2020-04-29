using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public partial class AppCommand
    {
        public class Config
        {
            public string Id { get; set; }
            public int Version { get; set; }
        }
    }
}
