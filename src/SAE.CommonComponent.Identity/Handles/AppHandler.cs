using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.Identity.Domains;
using SAE.CommonComponent.Identity.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System.Linq;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Caching;

namespace SAE.CommonComponent.Identity.Handles
{
    public class AppHandler : AbstractHandler,
                              ICommandHandler<AppCreateCommand, string>,
                              ICommandHandler<AppChangeCommand>,
                              ICommandHandler<AppChangeStatusCommand>,
                              ICommandHandler<AppCancelReferenceScopeCommand>,
                              ICommandHandler<AppReferenceScopeCommand>,
                              ICommandHandler<AppRefreshSecretCommand>,
                              ICommandHandler<AppQueryCommand, IPagedList<AppDto>>,
                              ICommandHandler<string, AppDto>,
                              ICommandHandler<ScopeCreateCommand>,
                              ICommandHandler<ScopeQueryALLCommand, IEnumerable<ScopeDto>>,
                              ICommandHandler<ScopeQueryCommand, IPagedList<ScopeDto>>,
                              ICommandHandler<ScopeRemoveCommand>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IStorage _storage;
        public string ScopeKey = nameof(ScopeKey);
        public AppHandler(IDocumentStore documentStore, IDistributedCache distributedCache, IStorage storage) : base(documentStore)
        {
            this._distributedCache = distributedCache;
            this._storage = storage;
        }


        public Task Handle(AppChangeCommand command)
        {
            return this.Update<App>(command.Id, app => app.Change(command));
        }

        public async Task<string> Handle(AppCreateCommand command)
        {
            var app = await this.Add(new App(command));
            return app.Id;
        }

        public Task Handle(AppChangeStatusCommand command)
        {
            return this.Update<App>(command.Id, app => app.ChangeStatus(command));
        }

        public Task Handle(AppCancelReferenceScopeCommand command)
        {
            return this.Update<App>(command.Id, app => app.CancelReference(command));
        }

        public Task Handle(AppReferenceScopeCommand command)
        {
            return this.Update<App>(command.Id, app => app.Reference(command));
        }

        public Task Handle(AppRefreshSecretCommand command)
        {
            return this.Update<App>(command.Id, app => app.RefreshSecret());
        }

        public Task<AppDto> Handle(string id)
        {

            return Task.FromResult(Assert.Build(this._storage.AsQueryable<AppDto>().FirstOrDefault(s => s.Id == id))
                                         .NotNull()
                                         .Current);
        }

        public Task<IPagedList<AppDto>> Handle(AppQueryCommand command)
        {
            var query = this._storage.AsQueryable<AppDto>();
            if (command.Name.IsNotNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            if (command.Url.IsNotNullOrWhiteSpace())
            {
                query = query.Where(s => s.Urls.Contains(command.Url));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task Handle(ScopeCreateCommand command)
        {
            var scopes = (await this._distributedCache.GetAsync<List<ScopeDto>>(ScopeKey)) ?? new List<ScopeDto>();
            if (!scopes.Any(s => s.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                scopes.Add(command.To<ScopeDto>());
                await this._distributedCache.AddAsync(ScopeKey, scopes, TimeSpan.FromDays(365 * 100));
            }
        }

        public async Task<IEnumerable<ScopeDto>> Handle(ScopeQueryALLCommand command)
        {
            return (await this._distributedCache.GetAsync<IEnumerable<ScopeDto>>(ScopeKey)).Distinct();
        }

        public async Task<IPagedList<ScopeDto>> Handle(ScopeQueryCommand command)
        {
            var scopes = await this._distributedCache.GetAsync<List<ScopeDto>>(ScopeKey);
            return PagedList.Build(scopes?.AsQueryable(), command);
        }

        public async Task Handle(ScopeRemoveCommand command)
        {
            var scopes = await this._distributedCache.GetAsync<List<ScopeDto>>(ScopeKey);
            if (scopes != null && scopes.Any(s => s.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                scopes.RemoveAll(s => s.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
                await this._distributedCache.AddAsync(ScopeKey, scopes, TimeSpan.FromDays(365 * 100));
            }
        }
    }
}