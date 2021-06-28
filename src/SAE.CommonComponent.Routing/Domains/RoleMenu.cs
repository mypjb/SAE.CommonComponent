using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Routing.Domains
{
    public class RoleMenu : Document,IEvent
    {
        public RoleMenu()
        {

        }

        public RoleMenu(string roleId, string menuId)
        {
            this.MenuId = menuId;
            this.RoleId = roleId;
            this.Id = $"{this.RoleId}_{this.MenuId}".ToMd5();
            this.Apply(this);
        }
        public string Id { get; set; }
        public string MenuId { get; set; }
        public string RoleId { get; set; }
    }
}
