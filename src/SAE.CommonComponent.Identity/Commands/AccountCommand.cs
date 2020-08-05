using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Commands
{
    /// <summary>
    /// 账号
    /// </summary>
    public class AccountLoginCommand
    {
        /// <summary>
        /// account name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// account password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// is persistent
        /// </summary>
        public bool Remember { get; set; }
        /// <summary>
        /// return url
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
