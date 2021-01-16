using SAE.CommonLibrary.EventStore;
using System;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class EnvironmentVariableEvent
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
        }
    }
}
