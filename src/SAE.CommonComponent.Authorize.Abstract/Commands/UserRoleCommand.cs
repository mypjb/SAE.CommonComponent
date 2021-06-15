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
            public string UserId { get; set; }
            public IEnumerable<string> Ids { get; set; }
        }
        public class DeleteRole : ReferenceRole
        {
        }
        
        public class QueryUserAuthorizeCode
        {
            public string UserId { get; set; }
        }
    }
}
