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
    public class AppRole : Document,IEvent
    {
        public AppRole()
        {

        }
        public AppRole(string appId,string roleId)
        {
            this.AppId = appId;
            this.RoleId = roleId;
            this.Id = $"{this.AppId}_{this.RoleId}".ToMd5();
            this.Apply(this);
        }
        public string Id { get; set; }
        public string AppId { get; set; }
        public string RoleId { get; set; }
    }
}
