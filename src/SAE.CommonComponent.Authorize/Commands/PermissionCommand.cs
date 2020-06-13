﻿using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class PermissionCommand
    {
        public class Create
        {
            public string Name { get; set; }
            public string Descriptor { get; set; }

            public string Path { get; set; }
        }


        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }

        public class Query : Paging
        {
            public string Name { get; set; }
        }

        public class Find
        {
            public string Id { get; set; }
        }

        public class List
        {

        }
    }
    

}
