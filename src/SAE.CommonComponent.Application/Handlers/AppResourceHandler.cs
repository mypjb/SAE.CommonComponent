using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;

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
        private readonly ILogging _logging;
        public AppResourceHandler(IDocumentStore documentStore,
                                  IStorage storage,
                                  IMediator mediator,
                                  ILogging<AppHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
        }


        public async Task HandleAsync(AppResourceCommand.Change command)
        {
            var appResource = await this._documentStore.FindAsync<AppResource>(command.Id);
            await appResource.NameExistAsync(this.FindAppResourceAsync);
            appResource.Change(command);
            await this._documentStore.SaveAsync(appResource);
        }

        public async Task<string> HandleAsync(AppResourceCommand.Create command)
        {
            var appResource = new AppResource(command);
            await appResource.NameExistAsync(this.FindAppResourceAsync);
            await this.AddAsync(appResource);
            return appResource.Id;
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

        private Task<AppResource> FindAppResourceAsync(AppResource appResource)
        {
            var oldAppResource = this._storage.AsQueryable<AppResource>()
                                   .FirstOrDefault(s => s.Name == appResource.Name &&
                                                        s.AppId == appResource.AppId &&
                                                        s.Id != appResource.Id);
            return Task.FromResult(oldAppResource);
        }
    }
}