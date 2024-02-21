using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent.User.Commands
{
    public class UserCommand
    {
        public class Create
        {
            /// <summary>
            /// user account name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// account password
            /// </summary>
            public string Password { get; set; }
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
            /// <summary>
            /// 
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Status Status { get; set; }

        }

        /// <summary>
        /// paging query
        /// </summary>
        public class Query : Paging
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Status Status { get; set; }
        }

        /// <summary>
        /// find user
        /// </summary>
        public class GetByName
        {
            /// <summary>
            /// 
            /// </summary>
            public string AccountName { get; set; }
        }

        ///<summary>
        ///
        ///</summary>
        public class Authentication
        {
            /// <summary>
            /// 
            /// </summary>
            public string AccountName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Password { get; set; }
        }
    }
}
