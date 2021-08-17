using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.BasicData.Commands
{
    public class DictCommand
    {
        public class Create
        {
            public string ParentId { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
        }

        public class Change 
        {
            public string ParentId { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
        }

       

        public class Query : Paging
        {
            public bool Root { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
        }

        public class List
        {
            public string Id { get; set; }
            public int Type { get; set; }
        }

        public class Tree
        {
            public string Id { get; set; }
            public int Type { get; set; }
        }
    }
}