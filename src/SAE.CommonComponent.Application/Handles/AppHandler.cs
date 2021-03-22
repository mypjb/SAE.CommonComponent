using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Caching;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Abstract.Handles
{
    public class AppHandler : AbstractHandler,
                              ICommandHandler<AppCommand.Create, string>,
                              ICommandHandler<AppCommand.Change>,
                              ICommandHandler<AppCommand.ChangeStatus>,
                              ICommandHandler<AppCommand.CancelReferenceScope>,
                              ICommandHandler<AppCommand.ReferenceScope>,
                              ICommandHandler<AppCommand.RefreshSecret>,
                              ICommandHandler<AppCommand.Query, IPagedList<AppDto>>,
                              ICommandHandler<Command.Find<AppDto>, AppDto>,
                              ICommandHandler<ScopeCommand.Create>,
                              ICommandHandler<Command.List<ScopeDto>, IEnumerable<ScopeDto>>,
                              ICommandHandler<ScopeCommand.Query, IPagedList<ScopeDto>>,
                              ICommandHandler<ScopeCommand.Delete>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IStorage _storage;
        private readonly ILogging<AppHandler> _logging;
        public string ScopeKey = nameof(ScopeKey);
        public AppHandler(IDocumentStore documentStore, 
                          IDistributedCache distributedCache, 
                          IStorage storage,
                          ILogging<AppHandler> logging) : base(documentStore)
        {
            this._distributedCache = distributedCache;
            this._storage = storage;
            this._logging = logging;
        }


        public Task Handle(AppCommand.Change command)
        {
            return this.Update<App>(command.Id, app => app.Change(command));
        }

        public async Task<string> Handle(AppCommand.Create command)
        {
            var app = await this.Add(new App(command));
            return app.Id;
        }

        public Task Handle(AppCommand.ChangeStatus command)
        {
            return this.Update<App>(command.Id, app => app.ChangeStatus(command));
        }

        public Task Handle(AppCommand.CancelReferenceScope command)
        {
            return this.Update<App>(command.Id, app => app.CancelReference(command));
        }

        public Task Handle(AppCommand.ReferenceScope command)
        {
            return this.Update<App>(command.Id, app => app.Reference(command));
        }

        public Task Handle(AppCommand.RefreshSecret command)
        {
            return this.Update<App>(command.Id, app => app.RefreshSecret());
        }

        public async Task<AppDto> Handle(Command.Find<AppDto> command)
        {
            var dto = this._storage.AsQueryable<AppDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
            
            this._logging.Info($"find app '{command.Id}',find '{dto?.ToJsonString()}' ");
            return dto;
        }

        public Task<IPagedList<AppDto>> Handle(AppCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task Handle(ScopeCommand.Create command)
        {
            var scopes = (await this._distributedCache.GetAsync<List<ScopeDto>>(ScopeKey)) ?? new List<ScopeDto>();
            if (!scopes.Any(s => s.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                scopes.Add(command.To<ScopeDto>());
                await this._distributedCache.AddAsync(ScopeKey, scopes, TimeSpan.FromDays(365 * 100));
            }
        }

        public async Task<IEnumerable<ScopeDto>> Handle(Command.List<ScopeDto> command)
        {
            return (await this._distributedCache.GetAsync<IEnumerable<ScopeDto>>(ScopeKey))?.Distinct() ?? Enumerable.Empty<ScopeDto>();
        }

        public async Task<IPagedList<ScopeDto>> Handle(ScopeCommand.Query command)
        {
            var scopes = await this._distributedCache.GetAsync<List<ScopeDto>>(ScopeKey);
            return PagedList.Build(scopes?.AsQueryable(), command);
        }

        public async Task Handle(ScopeCommand.Delete command)
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