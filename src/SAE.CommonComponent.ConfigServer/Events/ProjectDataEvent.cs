using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ProjectDataEvent
    {
        public class Create: ProjectData, IEvent
        {

        }
        public class Publish :IEvent
        {
            /// <summary>
            /// 版本号
            /// </summary>
            public int Version { get; set; }
            /// <summary>
            /// 项目已发布的配置数据
            /// </summary>
            public string Data { get; set; }
        }
    }
}
