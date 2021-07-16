using SAE.CommonComponent.Application.Dtos;
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
     
        public class ReferenceProject
        {
            public string Id { get; set; }
            public string[] ProjectIds { get; set; }
        }

        public class DeleteProject
        {
            public string Id { get; set; }
            public string[] ProjectIds { get; set; }
        }

        public class ReferenceScope
        {
            public string Id { get; set; }
            public string[] Scopes { get; set; }
        }

        public class DeleteScope
        {
            public string Id { get; set; }
            public string[] Scopes { get; set; }
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }

        public class ProjectQuery : Paging
        {
            /// <summary>
            /// app Id
            /// </summary>
            public string Id { get; set; }
            public string SolutionId { get; set; }
            public string Name { get; set; }
        }
    }
}