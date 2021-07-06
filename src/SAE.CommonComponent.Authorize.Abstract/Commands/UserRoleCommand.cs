using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class UserRoleCommand
    {
        public class ReferenceRole
        {
            public string Id { get; set; }
            public string[] RoleIds { get; set; }
        }
        public class DeleteRole : ReferenceRole
        {
        }
        
        public class QueryUserAuthorizeCode
        {
            public string UserId { get; set; }
        }

        public class Query:Paging
        {
            public string UserId { get; set; }
            public bool Referenced { get; set; }
        }
    }
}
