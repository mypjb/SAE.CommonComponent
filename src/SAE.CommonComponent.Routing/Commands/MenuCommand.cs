using System;
using System.Collections;
using System.Collections.Generic;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Routing.Commands
{
    public class MenuCreateCommand
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class MenuChangeCommand : MenuCreateCommand
    {
        public string Id { get; set; }
    }

    public class MenuQueryCommand : Paging
    {
        public string Name { get; set; } 
    }
}