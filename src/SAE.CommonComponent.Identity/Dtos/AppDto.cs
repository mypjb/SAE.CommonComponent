using System;
using System.Collections.Generic;
using SAE.CommonComponent.Identity.Domains;

namespace SAE.CommonComponent.Identity.Dtos
{
    public class AppDto
    {
        /// <summary>
        /// app id
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// app name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// app secret
        /// </summary>
        /// <value></value>
        public string Secret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Urls { get; set; }

        /// <summary>
        /// auth scope
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Scopes { get; set; }

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