using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent.User.Dtos
{
    public class AccountDto
    {
        public string Name { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AccountDto Account { get; set; }
        public Status Status { get; set; }
        /// <summary>
        /// 授权码，用于标识权限
        /// </summary>
        public string AuthorizeCode { get; set; }
        public DateTime CreateTime { get; set; }
       
    }
}
