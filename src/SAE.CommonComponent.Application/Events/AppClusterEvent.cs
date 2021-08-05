using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Events
{
    public partial class AppClusterEvent
    {
        public class Create : Change
        {
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