using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class ProjectConfigDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// config id
        /// </summary>
        public string ConfigId { get; set; }
        /// <summary>
        /// project id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 环境变量
        /// </summary>
        public string EnvironmentId { get; set; }
        /// <summary>
        /// Config
        /// </summary>
        public ConfigDto Config { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// private
        /// </summary>
        public bool Private { get; set; }
    }
}
