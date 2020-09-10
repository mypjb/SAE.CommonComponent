using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    public class UserRoleDto
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
