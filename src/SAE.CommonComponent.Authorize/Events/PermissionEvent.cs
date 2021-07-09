using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Events
{
    public class PermissionEvent
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

            public string Flag { get; set; }
            /// <summary>
            /// request method
            /// </summary>
            public AccessMethod Method { get; set; }
        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }
    }
}
