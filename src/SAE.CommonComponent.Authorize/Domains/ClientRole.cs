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
    /// 客户端认证凭据角色
    /// </summary>
    public class ClientRole : Document,IEvent
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public ClientRole()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="roleId"></param>
        public ClientRole(string clientId,string roleId)
        {
            this.ClientId = clientId;
            this.RoleId = roleId;
            this.Id = $"{this.ClientId}{this.RoleId}".ToMd5();
            this.Apply(this);
        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 客户端认证凭据标识
        /// </summary>
        /// <value></value>
        public string ClientId { get; set; }
        /// <summary>
        /// 角色标识
        /// </summary>
        /// <value></value>
        public string RoleId { get; set; }
    }
}
