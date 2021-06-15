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
            public string RoleId { get; set; }
            public IEnumerable<string> MenuIds { get; set; }
        }
        public class DeleteMenu : ReferenceMenu
        {
        }

        public class Query : Paging
        {
            public string RoleId { get; set; }
            public bool IgnoreRelevance { get; set; }
        }

    }
}
