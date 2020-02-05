using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class AbstractHandler<TDocument> where TDocument : IDocument, new()
    {
        protected readonly IDocumentStore _documentStore;
        protected readonly IStorage _storage;

        public AbstractHandler(IDocumentStore documentStore, IStorage storage)
        {
            this._documentStore = documentStore;
            this._storage = storage;
        }

        protected virtual async Task<TDocument> Add(TDocument document)
        {
            await this._documentStore.SaveAsync(document);
            return document;
        }

        protected virtual async Task Update(string identity,Action<TDocument> updateAction)
        {
            var document = await this._documentStore.FindAsync<TDocument>(identity);
            updateAction(document);
            await this._documentStore.SaveAsync(document);
        }

        protected virtual  Task Remove(string identity)
        {
            return this._documentStore.RemoveAsync<TDocument>(identity);
        }
    }
}
