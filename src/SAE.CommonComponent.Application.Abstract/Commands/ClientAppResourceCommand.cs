using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Commands
{
    public class ClientAppResourceCommand
    {
        public class ReferenceAppResource
        {
            public string ClientId { get; set; }
            public string[] AppResourceIds { get; set; }
        }
        public class DeleteAppResource : ReferenceAppResource
        {
        }


        public class Query : Paging
        {
            public string ClientId{ get; set; }
            public bool Referenced { get; set; }
        }
    }
}
