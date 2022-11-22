using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Dtos
{
    /// <summary>
    /// 集群下的应用
    /// </summary>
    public class AppDto
    {
        /// <summary>
        /// 系统标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 集群id
        /// </summary>
        public string ClusterId { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
    }
}
