using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Authorize.Commands
{
    public partial class RoleCommand
    {

        public class Create
        {
            public string Name { get; set; }
            public string Descriptor { get; set; }
        }

        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }
        public class RelationPermission
        {
            public string Id { get; set; }
            public IEnumerable<string> PermissionIds { get; set; }
        }

        public class DeletePermission
        {
            public string Id { get; set; }
            public IEnumerable<string> PermissionIds { get; set; }
        }
        public class Query : Paging
        {
            public string Name { get; set; }
        }

    }
}
