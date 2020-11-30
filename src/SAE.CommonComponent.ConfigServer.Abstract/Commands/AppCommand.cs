﻿using System;
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
            /// 配置标识
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 环境类型
            /// </summary>
            public string Env { get; set; }
            /// <summary>
            /// 版本
            /// </summary>
            public int Version { get; set; }
        }
    }
}
