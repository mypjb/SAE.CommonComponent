using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Domains
{
    public class ClientAppResource : Document, IEvent
    {
        public ClientAppResource()
        {

        }
        public ClientAppResource(string clientId, string appResourceId)
        {
            this.ClientId = clientId;
            this.AppResourceId = appResourceId;
            this.Id = $"{this.ClientId}_{this.AppResourceId}".ToMd5();
            this.Apply(this);
        }
        public string Id { get; set; }
        public string AppResourceId { get; set; }
        public string ClientId { get; set; }
    }
}
