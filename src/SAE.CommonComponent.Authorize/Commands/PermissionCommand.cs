using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{

    public class PermissionCreateCommand
    {
        public string Name { get; set; }
        public string Descriptor { get; set; }

        public string Path { get; set; }
    }


    public class PermissionChangeCommand: PermissionCreateCommand
    {
        public string Id { get; set; }
    }

    public class PermissionChangeStatusCommand
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }

    public class PermissionQueryCommand:Paging
    {
        public string Name { get; set; }
    }
    public class PermissionQueryALLCommand
    {

    }

}
