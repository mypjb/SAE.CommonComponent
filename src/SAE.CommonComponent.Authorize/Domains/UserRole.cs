﻿using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Domains
{
    public class UserRole:Document
    {
        public UserRole()
        {

        }
        public UserRole(string userId,string roleId)
        {
            this.UserId = userId;
            this.RoleId = roleId;
            this.Id = $"{this.UserId}_{this.RoleId}";
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}