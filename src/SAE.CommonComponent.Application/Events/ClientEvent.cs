using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Events
{
    public partial class ClientEvent
    {
        public class Create : Change
        {
            public string Id { get; set; }
            /// <summary>
            /// app id
            /// </summary>
            public string AppId { get; set; }
            public string Secret { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class Change : IEvent
        {
            public string Name { get; set; }
            public Endpoint Endpoint { get; set; }
            public string[] Scopes { get; set; }
        }

        public class RefreshSecret : IEvent
        {
            public string Secret { get; set; }
        }

        public class ChangeStatus : IEvent
        {
            public Status Status { get; set; }
        }

       
        public class ReferenceProject : IEvent
        {
            public string AppId { get; set; }
        }
    }
}