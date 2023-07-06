using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.MultiTenant.Domains
{
    /// <summary>
    /// 租户系统关联对象
    /// </summary>
    public class TenantApp : Document, IEvent
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public TenantApp()
        {
            
        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="tenantId">租户标识</param>
        /// <param name="appId">应用标识</param>
        public TenantApp(string tenantId, string appId)
        {
            this.TenantId = tenantId;
            this.AppId = appId;
            this.Id = $"{this.TenantId}{this.AppId}";
            this.Apply(this);
        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 租户标识
        /// </summary>
        /// <value></value>
        public string TenantId { get; set; }
        /// <summary>
        /// 应用标识
        /// </summary>
        /// <value></value>
        public string AppId { get; set; }
    }
}