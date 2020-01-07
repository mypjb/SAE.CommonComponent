using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class RemoveCommand<TDocument>
    {
        public string Id { get; set; }
    }

    public class GetByIdCommand<TDto>
    {
        public string Id { get; set; }
    }
}
