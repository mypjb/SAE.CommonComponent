using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Abstract.Events
{
    public partial class AppEvent
    {
        public class Create : IEvent
        {
            public string Id { get; set; }
            public string Secret { get; set; }
            public string Name { get; set; }
            public IEnumerable<string> Urls { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class Change : IEvent
        {
            public string Name { get; set; }
            public IEnumerable<string> Urls { get; set; }
        }

        public class RefreshSecret : IEvent
        {
            public string Secret { get; set; }
        }

        public class ReferenceScope : IEvent
        {
            public IEnumerable<string> Scopes { get; set; }
        }
        public class CancelReferenceScope : ReferenceScope
        {

        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }
    }
}