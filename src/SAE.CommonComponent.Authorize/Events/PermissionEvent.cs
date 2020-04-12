using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Events
{
    public class PermissionCreateEvent: PermissionChangeEvent
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Status Status { get; set; }
    }

    public class PermissionChangeEvent:IEvent
    {
        public string Name { get; set; }
        public string Descriptor { get; set; }

        public string Path { get; set; }
    }

    public class PermissionChangeStatusEvent : IEvent
    {
        public Status Status { get; set; }
    }

}
