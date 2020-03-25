using System;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Routing.Events
{
    public class MenuCreateEvent : MenuChangeEvent
    {
        public string Id { get; set; }
        
        public DateTime CreateTime { get; set; }
    }
    public class MenuChangeEvent : IEvent
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
