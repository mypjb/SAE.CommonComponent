using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ProjectCreateEvent : ProjectChangeEvent
    {
        public string SolutionId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Id { get; set; }
    }
    public class ProjectChangeEvent : IEvent
    {
        public string Name { get; set; }
    }

    public class ProjectRelevanceConfigEvent : IEvent
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ConfigId { get; set; }
        public string Alias { get; set; }
    }

    public class ProjectConfigChangeAliasEvent : IEvent
    {
        public string Alias { get; set; }
    }

    public class ProjectVersionCumulationEvent : IEvent
    {
        public int Version { get; set; }
    }

}
