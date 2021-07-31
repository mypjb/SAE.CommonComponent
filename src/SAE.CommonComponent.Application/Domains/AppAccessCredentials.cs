using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Domains
{
    public class AppAccessCredentials:Document,IEvent
    {
        public AppAccessCredentials()
        {

        }
        public AppAccessCredentials(string appId, string accessId)
        {
            this.AppId = appId;
            this.AccessId = accessId;
            this.Id = $"{this.AppId}_{this.AccessId}".ToMd5();
            this.Apply(this);
        }
        public string Id { get; set; }
        public string AppId { get; set; }
        public string AccessId { get; set; }
    }
}
