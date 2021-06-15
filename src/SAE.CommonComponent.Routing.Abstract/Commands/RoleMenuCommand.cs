using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Routing.Abstract.Commands
{
    public class RoleMenuCommand
    {
        public class ReferenceMenu
        {
            public string UserId { get; set; }
            public IEnumerable<string> Ids { get; set; }
        }
        public class DeleteMenu : ReferenceMenu
        {
        }

        public class Query : Paging
        {
            public string RoleId { get; set; }
            public bool HasRelevance { get; set; }
        }

    }
}
