using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public Status Status { get; set; }
        public string[] PermissionIds { get; set; }
        public PermissionDto[] Permissions { get; set; }
        public string[] MenuIds { get; set; }
    }
}
