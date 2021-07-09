using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Events
{
    public partial class RoleEvent
    {
        public class Create : Change
        {
            public string Id { get; set; }
            public DateTime CreateTime { get; set; }
            public Status Status { get; set; }
        }

        public class Change : IEvent
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }

        public class RelevancePermission : IEvent
        {
            public string[] PermissionIds { get; set; }
        }

        public class RelevanceMenu : IEvent
        {
            public string[] MenuIds { get; set; }
        }
    }
}
