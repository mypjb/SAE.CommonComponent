using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.PluginManagement.Commands
{
    public class PluginCommand
    {
        public class Create
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Version { get; set; }
            public int Order { get; set; }
            public Status Status { get; set; }
           
        }

        public class Change
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Version { get; set; }
            public int Order { get; set; }
        }

        public class ChangeEntry
        {
            public string Id { get; set; }
            public string Path { get; set; }
            public string Entry { get; set; }
        }
        public class Query : Paging
        {
            public string Name { get; set; }
        }

        public class ChangeStatus
        {
            public string Id { get; set; }
            public Status Status { get; set; }
        }
    }
}
