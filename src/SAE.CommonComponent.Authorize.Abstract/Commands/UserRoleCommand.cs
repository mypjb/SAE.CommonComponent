using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class UserRoleCommand
    {
        public class Reference
        {
            public string UserId { get; set; }
            public IEnumerable<string> Ids { get; set; }
        }
        public class DeleteReference : Reference
        {
        }
        
        public class QueryUserAuthorizeCode
        {
            public string UserId { get; set; }
        }
    }
}
