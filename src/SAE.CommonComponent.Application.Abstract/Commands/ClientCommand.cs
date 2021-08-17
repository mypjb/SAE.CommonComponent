using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Commands
{
    public class ClientCommand
    {
        public class Query : Paging
        {
            public string AppId { get; set; }
            public string Name { get; set; }
        }
        public class Create : Change
        {
            public string AppId { get; set; }
            public string Secret { get; set; }
        }
        public class Change
        {
            public string Id { get; set; }

            public string Name { get; set; }
            public EndpointDto Endpoint { get; set; }
            public string[] Scopes { get; set; }
        }
        public class RefreshSecret
        {
            public string Id { get; set; }
        }

       
        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }

    }
}