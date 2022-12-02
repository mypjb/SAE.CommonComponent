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
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRole:Document,IEvent
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public UserRole()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        public UserRole(string userId,string roleId)
        {
            this.UserId = userId;
            this.RoleId = roleId;
            this.Id = $"{this.UserId}{this.RoleId}".ToMd5();
            this.Apply(this);
        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        /// <value></value>
        public string UserId { get; set; }
        /// <summary>
        /// 角色标识
        /// </summary>
        /// <value></value>
        public string RoleId { get; set; }
    }
}
