using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Authorize.Commands
{
    public class PermissionCommand
    {
        public class Create
        {
            public string Name { get; set; }
            public string Descriptor { get; set; }

            public string Flag { get; set; }
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

    }
    

}
