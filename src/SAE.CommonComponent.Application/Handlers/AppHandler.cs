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

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AppHandler : AbstractHandler,
                              ICommandHandler<AppCommand.Create, string>,
                              ICommandHandler<AppCommand.Change>,
                              ICommandHandler<AppCommand.ChangeStatus>,
                              ICommandHandler<AppCommand.CancelReferenceScope>,
                              ICommandHandler<AppCommand.ReferenceScope>,
                              ICommandHandler<AppCommand.RefreshSecret>,
                              ICommandHandler<AppCommand.Query, IPagedList<AppDto>>,
                              ICommandHandler<Command.Find<AppDto>, AppDto>
    {
        private readonly IStorage _storage;
        private readonly ILogging<AppHandler> _logging;
        public AppHandler(IDocumentStore documentStore,
                          IStorage storage,
                          ILogging<AppHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._logging = logging;
        }


        public Task HandleAsync(AppCommand.Change command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.Change(command));
        }

        public async Task<string> HandleAsync(AppCommand.Create command)
        {
            var app = await this.AddAsync(new App(command));
            return app.Id;
        }

        public Task HandleAsync(AppCommand.ChangeStatus command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.ChangeStatus(command));
        }

        public Task HandleAsync(AppCommand.CancelReferenceScope command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.CancelReference(command));
        }

        public Task HandleAsync(AppCommand.ReferenceScope command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.Reference(command));
        }

        public Task HandleAsync(AppCommand.RefreshSecret command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.RefreshSecret());
        }

        public async Task<AppDto> HandleAsync(Command.Find<AppDto> command)
        {
            var dto = this._storage.AsQueryable<AppDto>()
                          .FirstOrDefault(s => s.Id == command.Id);

            this._logging.Info($"find app '{command.Id}',find '{dto?.ToJsonString()}' ");
            return dto;
        }

        public Task<IPagedList<AppDto>> HandleAsync(AppCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

    }
}