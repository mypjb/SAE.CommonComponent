using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ProjectEvent
    {
        public class Create : Change
        {
            public string SolutionId { get; set; }
            public DateTime CreateTime { get; set; }
            public string Id { get; set; }
        }
        public class Change : IEvent
        {
            public string Name { get; set; }
        }

        public class RelevanceConfig : IEvent
        {
            public string Id { get; set; }
            public string ProjectId { get; set; }
            public string ConfigId { get; set; }
            public string Alias { get; set; }
            public string Env { get; set; }
        }

        public class ConfigChangeAlias : IEvent
        {
            public string Alias { get; set; }
        }

        public class VersionCumulation : IEvent
        {
            public int Version { get; set; }
        }
    }
}
