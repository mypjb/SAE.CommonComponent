using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class EnvironmentVariableCommand
    {
        public class Create
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
        }
        public class Change
        {
            public string Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
        }

    }
}
