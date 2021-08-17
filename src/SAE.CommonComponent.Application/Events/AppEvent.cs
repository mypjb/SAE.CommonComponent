using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Events
{
    public partial class AppEvent
    {
        public class Create : Change
        {
            /// <summary>
            /// cluster id
            /// </summary>
            public string ClusterId { get; set; }
            public string Id { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class Change : IEvent
        {
            public string Name { get; set; }
        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }
    }
}