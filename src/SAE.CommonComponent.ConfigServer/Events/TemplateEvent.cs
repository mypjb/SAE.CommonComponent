using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class TemplateEvent
    {
        public class Create : Change
        {
            public string Id { get; set; }
            public DateTime CreateTime { get; set; }
        }
        public class Change : IEvent
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 格式
            /// </summary>
            public string Format { get; set; }
        }
    }
}
