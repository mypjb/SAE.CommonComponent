using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Handlers
{
    public class ClientAppResourceHandler : AbstractHandler<ClientAppResource>,
                                                       ICommandHandler<ClientAppResourceCommand.ReferenceAppResource>,
                                                       ICommandHandler<ClientAppResourceCommand.DeleteAppResource>,
                                                       ICommandHandler<ClientAppResourceCommand.Query,IPagedList<AppResourceDto>>
    {
        private readonly IStorage _storage;

        public ClientAppResourceHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task<IPagedList<AppResourceDto>> HandleAsync(ClientAppResourceCommand.Query command)
        {
            if (command.Referenced)
            {
                var paging = PagedList.Build(this._storage.AsQueryable<ClientAppResource>()
                                   .Where(s => s.ClientId == command.ClientId), command);

                var ids = paging.Select(ar => ar.AppResourceId).ToArray();

                return PagedList.Build(this._storage.AsQueryable<AppResourceDto>()
                                                    .Where(s => ids.Contains(s.Id))
                                                    .ToList(), paging);

            }
            else
            {
                var ids = this._storage.AsQueryable<ClientAppResource>()
                                       .Where(s => s.ClientId == command.ClientId)
                                       .Select(s => s.AppResourceId)
                                       .ToArray();

                return PagedList.Build(this._storage.AsQueryable<AppResourceDto>()
                                           .Where(s => !ids.Contains(s.Id)), command);
            }
        }

        public async Task HandleAsync(ClientAppResourceCommand.ReferenceAppResource command)
        {
            var accessCredentialsAppResources = command.AppResourceIds.Select(s => new ClientAppResource(command.ClientId, s))
                                                                      .ToArray();
            await this._documentStore.SaveAsync(accessCredentialsAppResources);            
        }

        public async Task HandleAsync(ClientAppResourceCommand.DeleteAppResource command)
        {
            var accessCredentialsAppResources = command.AppResourceIds.Select(s => new ClientAppResource(command.ClientId, s))
                                                  .ToArray();

            await this._documentStore.DeleteAsync(accessCredentialsAppResources);
        }
    }
}
