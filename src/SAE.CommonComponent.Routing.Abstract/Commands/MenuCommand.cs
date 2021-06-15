using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Routing.Commands
{
    public class MenuCommand
    {
        public class Create
        {
            public string ParentId { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class RelevancePermission
        {
            /// <summary>
            /// menu Id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// permission id list 
            /// </summary>
            public IEnumerable<string> PermissionIds { get; set; }
        }

        public class DeletePermission: RelevancePermission
        {
        }

        public class PermissionQuery:Paging
        {
            public string Id { get; set; }
            public bool HasRelevance { get; set; }
        }

        public class Query : Paging
        {
            public string Name { get; set; }
        }

    }
}