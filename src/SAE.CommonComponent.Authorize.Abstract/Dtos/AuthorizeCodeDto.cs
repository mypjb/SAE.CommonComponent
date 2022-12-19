using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    public class AuthorizeCodeDto
    {
        /// <summary>
        /// 超级管理员
        /// </summary>
        /// <value></value>
        public IEnumerable<string> SuperAdmins { get; set; }
        /// <summary>
        /// 系统授权码
        /// </summary>
        /// <value></value>
        public IDictionary<string, string> Codes { get; set; }
    }
}