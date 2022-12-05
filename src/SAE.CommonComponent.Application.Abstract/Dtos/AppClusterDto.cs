using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Dtos
{
    /// <summary>
    /// 应用集群
    /// </summary>
    public class AppClusterDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 集群名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 集群类型。（集群下的资源属于多租户时，应该设置该值，该值对应字典标识，一旦设置不可更改!)
        /// </summary>
        /// <value></value>
        public string Type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
        /// <summary>
        /// 集群设置(当集群类型存在时，应该设置该对象)
        /// </summary>
        /// <value></value>
        public AppClusterSettingDto Setting { get; set; }
    }
}
