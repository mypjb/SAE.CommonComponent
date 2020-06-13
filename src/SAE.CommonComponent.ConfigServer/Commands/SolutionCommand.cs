using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class SolutionCommand
    {
        public class Create
        {
            public string Name { get; set; }
        }
        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class Query : Paging
        {
            public string Name { get; set; }
        }

        public class Find
        {
            public string Id { get; set; }
        }
    }
}
