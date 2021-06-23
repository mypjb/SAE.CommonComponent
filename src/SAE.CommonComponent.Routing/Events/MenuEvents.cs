using System;
using System.Collections.Generic;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.Routing.Events
{
    public class MenuEvent
    {
        public class Create : Change
        {
            public string Id { get; set; }

            public DateTime CreateTime { get; set; }
        }
        public class Change : IEvent
        {
            public string ParentId { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }

            public bool Hidden { get; set; }
        }

        public class RelevancePermission : IEvent
        {
            public IEnumerable<string> PermissionIds { get; set; }
        }
    }
}
