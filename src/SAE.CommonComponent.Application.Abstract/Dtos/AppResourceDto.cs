using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Dtos
{
    /// <summary>
    /// 系统下的资源
    /// </summary>
    public class AppResourceDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 资源索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 资源名词
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 资源访问地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 资源请求谓词 (get、post、put...)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
