using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class AppRoleCommand
    {
        public class ReferenceRole
        {
            public string AppId { get; set; }
            public string[] RoleIds { get; set; }
        }
        public class DeleteRole : ReferenceRole
        {
        }
        
        public class QueryAppAuthorizeCode
        {
            public string AppId { get; set; }
        }

        public class Query:Paging
        {
            public string AppId { get; set; }
            public bool Referenced { get; set; }
        }
    }
}
