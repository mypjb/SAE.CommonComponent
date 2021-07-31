using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Commands
{
    public class AppCommand
    {
        public class Query : Paging
        {
            public string Name { get; set; }
        }
        public class Create
        {
            public string Name { get; set; }
        }
        public class Change: Create
        {
            public string Id { get; set; }

            
        }

        public class ReferenceProject
        {
            public string Id { get; set; }
            public string ProjectId { get; set; }
        }

        public class DeleteProject : ReferenceProject
        {
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }

        public class ProjectQuery : Paging
        {
            /// <summary>
            /// app Id
            /// </summary>
            public string Id { get; set; }
            public bool Referenced { get; set; }
            public string Name { get; set; }
        }
    }
}