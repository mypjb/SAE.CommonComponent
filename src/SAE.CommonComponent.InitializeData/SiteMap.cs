using SAE.CommonComponent.Routing.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public class SiteMap:MenuItemDto
    {
        public string Entry { get; set; }
        public string Plugin { get; set; }
    }
}
