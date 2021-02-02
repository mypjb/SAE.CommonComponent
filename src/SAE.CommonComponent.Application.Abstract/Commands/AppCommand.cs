using SAE.CommonComponent.Application.Abstract.Dtos;
using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Commands
{
    public class AppCommand
    {
        public class Query : Paging
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
        public class Create
        {
            public string Id { get; set; }
            public string Secret { get; set; }
            public string Name { get; set; }
            public EndpointDto Endpoint { get; set; }
        }
        public class Change : Create
        {
            //public string Id { get; set; }
        }
        public class RefreshSecret
        {
            public string Id { get; set; }
        }

        public class ReferenceScope
        {
            public string Id { get; set; }
            public IEnumerable<string> Scopes { get; set; }
        }

        public class CancelReferenceScope
        {
            public string Id { get; set; }
            public IEnumerable<string> Scopes { get; set; }
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }
    }
}