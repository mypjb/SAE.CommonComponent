using SAE.CommonLibrary.EventStore;
using System;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class SolutionCreateEvent : SolutionChangeEvent
    {
        public DateTime CreateTime { get; set; }
        public string Id { get;  set; }
    }
    public class SolutionChangeEvent : IEvent
    {
        public string Name { get; set; }
    }
}
