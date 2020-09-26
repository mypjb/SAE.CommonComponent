namespace SAE.CommonComponent.Routing.Commands
{
    public class MenuCommand
    {
        public class Create
        {
            public string ParentId { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class Change : Create
        {
            public string Id { get; set; }
        }

        // public class MenuQueryCommand : Paging
        // {
        //     public string Name { get; set; }
        // }

    }
}