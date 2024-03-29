using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary.Abstract.Model;
using System.Collections.Generic;

namespace SAE.CommonComponent.Routing.Commands
{
    public class MenuCommand
    {
        public class Create
        {
            public string AppId { get; set; }
            public string ParentId { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }

            public bool Hidden { get; set; }
        }

        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class ReferencePermission
        {
            /// <summary>
            /// menu Id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// permission id list 
            /// </summary>
            public string[] PermissionIds { get; set; }
        }

        public class DeletePermission: ReferencePermission
        {
        }

        public class PermissionQuery:Paging
        {
            public string Id { get; set; }
            public bool Referenced { get; set; }
        }

        public class Query : Paging
        {
            public string Name { get; set; }
            public string[] IgnoreIds { get; set; }
        }

        public class Tree
        {

        }

        public class Finds
        {
            public string[] Ids { get; set; }
        }
    }
}