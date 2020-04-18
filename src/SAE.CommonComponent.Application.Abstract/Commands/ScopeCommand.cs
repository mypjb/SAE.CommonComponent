using System.Collections;
using System.Collections.Generic;
using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Application.Abstract.Commands
{
    public class ScopeCreateCommand
    {
        public string Name { get; set; }
        public string Display { get; set; }
    }

    public class ScopeRemoveCommand 
    {
        public string Name { get; set; }
    }
    public class ScopeQueryCommand : Paging
    {

    }
    public class ScopeQueryALLCommand
    {
    }
}