using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Dtos
{
    public class AppResourceDto
    {
        /// <summary>
        /// identity
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// resource index relative to the app
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// resource name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// resource path
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// resource method (get、post、put...)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
