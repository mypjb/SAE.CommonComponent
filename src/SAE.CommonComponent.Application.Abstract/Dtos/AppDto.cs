using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Dtos
{
    public class AppDto
    {
        /// <summary>
        /// system id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// cluster id
        /// </summary>
        public string ClusterId { get; set; }
        /// <summary>
        /// system name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// app status
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
    }
}
