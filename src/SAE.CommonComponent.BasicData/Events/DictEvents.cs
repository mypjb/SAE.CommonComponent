using System;
using System.Collections.Generic;
using SAE.CommonLibrary.EventStore;

namespace SAE.CommonComponent.BasicData.Events
{
    public class DictEvent
    {
        public class Create : Change
        {
            public string Id { get; set; }
            public DateTime CreateTime { get; set; }
            public int Type { get; set; }
        }
        public class Change : IEvent
        {
            public string ParentId { get; set; }
            public string Name { get; set; }
        }

    }
}
