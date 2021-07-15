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
        /// <summary>
        /// Once the solution is assigned, it will not change
        /// </summary>
        public class DistributionSolution
        {
            public string Id { get; set; }
            public string SolutionId { get; set; }
        }

        public class RelevanceAppConfig
        {
            public string Id { get; set; }
            public string[] AppConfigIds { get; set; }
        }

        public class DeleteAppConfig
        {
            public string Id { get; set; }
            public string[] AppConfigIds { get; set; }
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
    }
}