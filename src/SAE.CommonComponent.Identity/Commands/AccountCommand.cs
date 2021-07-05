using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Commands
{
    public class AccountCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public class Login
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

        /// <summary>
        /// 
        /// </summary>
        public class Register
        {
            /// <summary>
            /// user account name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// account password
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// confirm password
            /// </summary>
            public string ConfirmPassword { get; set; }
        }
    }
}
