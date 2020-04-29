using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent.User.Commands
{
    public class UserCommand
    {
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

        /// <summary>
        /// 
        /// </summary>
        public class ChangePassword
        {
            /// <summary>
            /// user id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// original password
            /// </summary>
            public string OriginalPassword { get; set; }
            /// <summary>
            /// change password
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// confirm password
            /// </summary>
            public string ConfirmPassword { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }

        }


        /// <summary>
        /// 
        /// </summary>
        public class Query : Paging
        {
            public string Name { get; set; }
            public Status Status { get; set; }
        }

    }
}
