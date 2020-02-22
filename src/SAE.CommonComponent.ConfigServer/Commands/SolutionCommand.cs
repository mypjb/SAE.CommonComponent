using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class SolutionCreateCommand
    {
        public string Name { get; set; }
    }
    public class SolutionChangeCommand : SolutionCreateCommand
    {
        public string Id { get; set; }
    }

    public class SolutionQueryCommand : Paging
    {
        public string Name { get; set; }
    }

}
