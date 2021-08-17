using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Domains
{
    public class ClientRole : Document,IEvent
    {
        public ClientRole()
        {

        }
        public ClientRole(string clientId,string roleId)
        {
            this.ClientId = clientId;
            this.RoleId = roleId;
            this.Id = $"{this.ClientId}_{this.RoleId}".ToMd5();
            this.Apply(this);
        }
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string RoleId { get; set; }
    }
}
