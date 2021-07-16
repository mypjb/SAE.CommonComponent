using SAE.CommonComponent.Application.Abstract.Domains;
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
            public DateTime CreateTime { get; set; }
            public Endpoint Endpoint { get; set; }
        }

        public class Change : IEvent
        {
            public string Name { get; set; }
            public string[] Urls { get; set; }
        }

        public class RefreshSecret : IEvent
        {
            public string Secret { get; set; }
        }

        public class ReferenceScope : IEvent
        {
            public string[] Scopes { get; set; }
        }
        public class DeleteScope : ReferenceScope
        {

        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }

       
        public class ReferenceProject : IEvent
        {
            public string[] ProjectIds { get; set; }
        }
        public class DeleteProject : ReferenceProject
        {

        }
    }
}