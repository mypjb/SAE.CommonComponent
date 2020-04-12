﻿using System;
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
        public string Path { get; set; }
        public DateTime CreateTime { get; set; }
        public Status Status { get; set; }
    }
}
