using SAE.CommonComponent.Routing.Domains;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Routing.Handlers
{
    public class RoleMenuHandler : AbstractHandler<RoleMenu>
    {
        public RoleMenuHandler(IDocumentStore documentStore) : base(documentStore)
        {
        }
    }
}
