using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Application.Commands
{
    public class ScopeCommand
    {

        public class Create
        {
            public string Name { get; set; }
            public string Display { get; set; }
        }

        public class Remove
        {
            public string Name { get; set; }
        }
        public class Query : Paging
        {

        }
        public class QueryALL
        {
        }
    }
}