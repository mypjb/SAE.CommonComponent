using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class RemoveCommand<TDocument> where TDocument : IDocument
    {
        public string Id { get; set; }
    }
    public class BatchRemoveCommand<TDocument> where TDocument : IDocument
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class GetByIdCommand<TDto>
    {
        public string Id { get; set; }
    }
}
