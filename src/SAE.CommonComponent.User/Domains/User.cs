using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.User.Domains
{
    public class Account
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Slat { get; set; }
    }
    public class User : Document
    {
       

        public User()
        {

        }
        /// <summary>
        /// user id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// user name 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// user account
        /// </summary>
        public Account Account { get; set; }
        /// <summary>
        /// user status 
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// create create time
        /// </summary>
        public DateTime CrateTime { get; set; }
    }
}
