using System.Collections;
using System.Collections.Generic;
using SAE.CommonComponent.Identity.Domains;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Identity.Commands
{
    public class ScopeCreateCommand
    {
        public string Name { get; set; }
        public string Display { get; set; }
    }

    public class ScopeRemoveCommand : ScopeCreateCommand
    {
    }
    public class ScopeQueryCommand : Paging
    {

    }
    public class ScopeQueryALLCommand
    {
    }
}