using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Events
{
    public class RoleCreateEvent : RoleChangeEvent
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Status Status { get; set; }
    }

    public class RoleChangeEvent : IEvent
    {
        public string Name { get; set; }
        public string Descriptor { get; set; }
    }

    public class RoleChangeStatusEvent : IEvent
    {
        public Status Status { get; set; }
    }

    public class RoleRelationPermissionEvent : IEvent
    {
        public IEnumerable<string> PermissionIds { get; set; }
    }
}
