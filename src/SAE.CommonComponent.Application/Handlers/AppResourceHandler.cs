using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AppResourceHandler : AbstractHandler,
                              ICommandHandler<AppResourceCommand.Create, string>,
                              ICommandHandler<AppResourceCommand.Change>,
                              ICommandHandler<Command.Delete<AppResource>>,
                              ICommandHandler<AppResourceCommand.Query, IPagedList<AppResourceDto>>,
                              ICommandHandler<Command.Find<AppResourceDto>, AppResourceDto>,
                              ICommandHandler<AppResourceCommand.List, IEnumerable<AppResourceDto>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly ILogging<AppHandler> _logging;
        public AppResourceHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IMediator mediator,
                          ILogging<AppHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
        }


        public Task HandleAsync(AppResourceCommand.Change command)
        {
            return this.UpdateAsync<AppResource>(command.Id, app => app.Change(command));
        }

        public async Task<string> HandleAsync(AppResourceCommand.Create command)
        {
            var app = await this.AddAsync(new AppResource(command));
            return app.Id;
        }


        public async Task<AppResourceDto> HandleAsync(Command.Find<AppResourceDto> command)
        {
            var dto = this._storage.AsQueryable<AppResourceDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public Task<IPagedList<AppResourceDto>> HandleAsync(AppResourceCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppResourceDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task HandleAsync(Command.Delete<AppResource> command)
        {
            await this.DeleteAsync<AppResource>(command.Id);
        }

        public Task<IEnumerable<AppResourceDto>> HandleAsync(AppResourceCommand.List command)
        {
            return Task.FromResult(this._storage.AsQueryable<AppResourceDto>()
                 .Where(s => s.AppId == command.AppId)
                 .ToArray()
                 .AsEnumerable());
        }
    }
}