using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class PermissionCommand
    {
        public class Create
        {
            public string AppId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

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
            public string[] IgnoreIds { get; set; }
        }

        public class Finds
        {
            public string[] Ids { get; set; }
        }

    }
    

}
