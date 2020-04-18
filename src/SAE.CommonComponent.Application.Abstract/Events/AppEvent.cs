using System.Collections.Generic;
using SAE.CommonLibrary.EventStore;
using System;
using SAE.CommonComponent.Application.Abstract.Domains;

namespace SAE.CommonComponent.Application.Abstract.Events
{
    public class AppCreateEvent : IEvent
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Urls { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class AppChangeEvent : IEvent
    {
        public string Name { get; set; }
        public IEnumerable<string> Urls { get; set; }
    }

    public class AppRefreshSecretEvent : IEvent
    {
        public string Secret { get; set; }
    }

    public class AppReferenceScopeEvent : IEvent
    {
        public IEnumerable<string> Scopes { get; set; }
    }
    public class AppCancelReferenceScopeEvent : AppReferenceScopeEvent
    {

    }

    public class AppChangeStatusEvent : IEvent
    {
        public Status Status { get; set; }
    }
}