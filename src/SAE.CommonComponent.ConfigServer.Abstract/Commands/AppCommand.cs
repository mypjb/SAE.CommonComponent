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
            /// <summary>
            /// app id
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 环境类型
            /// </summary>
            public string Env { get; set; }
            /// <summary>
            /// 版本
            /// </summary>
            public int Version { get; set; }
            /// <summary>
            /// private
            /// </summary>
            public bool Private { get; set; }
        }
    }
}
