using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class ClientRoleCommand
    {
        public class ReferenceRole
        {
            public string ClientId { get; set; }
            public string[] RoleIds { get; set; }
        }
        public class DeleteRole : ReferenceRole
        {
        }
        
        public class QueryClientAuthorizeCode
        {
            public string ClientId { get; set; }
        }

        public class Query:Paging
        {
            public string ClientId { get; set; }
            public bool Referenced { get; set; }
        }
    }
}
