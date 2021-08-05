using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Commands
{
    public class AppCommand
    {
        public class Query : Paging
        {
            public string ClusterId { get; set; }
            public string Name { get; set; }
        }
        public class Create
        {
            public string Id { get; set; }
            public string ClusterId { get; set; }
            public string Name { get; set; }
        }
        public class Change: Create
        {
            public string Id { get; set; }
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }
    }
}