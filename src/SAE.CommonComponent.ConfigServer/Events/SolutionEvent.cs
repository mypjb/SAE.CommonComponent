using SAE.CommonLibrary.EventStore;
using System;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class SolutionEvent
    {
        public class Create : Change
        {
            public DateTime CreateTime { get; set; }
            public string Id { get; set; }
        }
        public class Change : IEvent
        {
            public string Name { get; set; }
        }
    }
}
