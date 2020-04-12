using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class RoleCreateCommand
    {
        public string Name { get; set; }
        public string Descriptor { get; set; }
    }

    public class RoleChangeCommand : RoleCreateCommand
    {
        public string Id { get; set; }
    }

    public class RoleChangeStatusCommand
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }
    public class RoleRelationPermissionCommand
    {
        public string Id { get; set; }
        public IEnumerable<string> PermissionIds { get; set; }
    }
    public class RoleQueryCommand : Paging
    {
        public string Name { get; set; }
    }

    public class RoleQueryALL
    {

    }
}
