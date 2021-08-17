using SAE.CommonLibrary.EventStore;
using System;

namespace SAE.CommonComponent.PluginManagement.Events
{
    public class PluginEvent
    {
        public class Create:IEvent
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Version { get; set; }

            public int Order { get; set; }

            public string Path { get; set; }
            public string Entry { get; set; }

            public Status Status { get; set; }

            public DateTime CreateTime { get; set; }
        }

        public class Change : IEvent
        {
            public string Description { get; set; }
            public string Version { get; set; }

            public int Order { get; set; }
        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }

        public class ChangeEntry : IEvent
        {
            public string Entry { get; set; }
            public string Path { get; set; }
        }
    }
}
