using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Dtos
{
    public class AppConfigDataPreviewDto
    {
        /// <summary>
        /// Public data
        /// </summary>
        public object Public { get; set; }
        /// <summary>
        /// Private data
        /// </summary>
        public object Private { get; set; }
    }
}
