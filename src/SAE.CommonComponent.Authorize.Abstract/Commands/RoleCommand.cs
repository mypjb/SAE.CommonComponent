using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Authorize.Commands
{
    public partial class RoleCommand
    {

        public class Create
        {
            public string Name { get; set; }
            public string Description { get; set; }
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
        public class ReferencePermission
        {
            public string Id { get; set; }
            public string[] PermissionIds { get; set; }
        }

        public class DeletePermission : ReferencePermission
        {
        }

        public class ReferenceMenu
        {
            public string Id { get; set; }
            public string[] MenuIds { get; set; }
        }

        public class DeleteMenu : ReferenceMenu
        {
        }
        public class Query : Paging
        {
            public string Name { get; set; }
        }

        public class PermissionQuery : Paging
        {
            /// <summary>
            /// role id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Referenced { get; set; }
        }
    }
}
