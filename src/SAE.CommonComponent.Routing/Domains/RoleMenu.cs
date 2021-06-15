using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Routing.Domains
{
    public class RoleMenu:Document
    {
        public RoleMenu()
        {

        }

        public RoleMenu(string menuId,string roleId)
        {
            this.Id = $"{roleId}_{menuId}";
            this.MenuId = menuId;
            this.RoleId = roleId;
        }
        public string Id { get; set; }
        public string MenuId { get; set; }
        public string RoleId { get; set; }
    }
}
