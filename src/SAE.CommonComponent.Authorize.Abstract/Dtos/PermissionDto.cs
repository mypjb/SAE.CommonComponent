using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Dtos
{
    public class PermissionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Descriptor { get; set; }
        public string Flag { get; set; }
        public AccessMethod Method { get; set; }
        public DateTime CreateTime { get; set; }
        public Status Status { get; set; }
    }
}
