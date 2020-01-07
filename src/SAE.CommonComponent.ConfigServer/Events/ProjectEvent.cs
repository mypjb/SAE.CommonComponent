using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ProjectCreateEvent : ProjectChangeEvent
    {
        public string SolutionId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Id { get;  set; }
    }
    public class ProjectChangeEvent : IEvent
    {
        public string Name { get; set; }
        public int Version { get; set; }
    }

    public class ProjectRelevanceConfigEvent : IEvent
    {
        public IEnumerable<string> ConfigIds { get; set; }
    }
}
