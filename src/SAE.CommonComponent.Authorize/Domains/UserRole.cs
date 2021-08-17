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
    public class UserRole:Document,IEvent
    {
        public UserRole()
        {

        }
        public UserRole(string userId,string roleId)
        {
            this.UserId = userId;
            this.RoleId = roleId;
            this.Id = $"{this.UserId}_{this.RoleId}".ToMd5();
            this.Apply(this);
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
