using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System.Linq;
using System.Threading.Tasks;
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AppHandler : AbstractHandler,
                              ICommandHandler<AppCommand.Create, string>,
                              ICommandHandler<AppCommand.Change>,
                              ICommandHandler<Command.Delete<App>>,
                              ICommandHandler<AppCommand.ChangeStatus>,
                              ICommandHandler<AppCommand.Query, IPagedList<AppDto>>,
                              ICommandHandler<Command.Find<AppDto>, AppDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly ILogging<AppHandler> _logging;
        public AppHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IMediator mediator,
                          ILogging<AppHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
        }


        public async Task HandleAsync(AppCommand.Change command)
        {
            var app = await this._documentStore.FindAsync<App>(command.Id);
            await app.NameExistAsync(this.FindAppAsync);
            app.Change(command);
            await this._documentStore.SaveAsync(app);
        }

        public async Task<string> HandleAsync(AppCommand.Create command)
        {
            var app = new App(command);
            await app.NameExistAsync(this.FindAppAsync);
            await this.AddAsync(app);
            return app.Id;
        }

        public Task HandleAsync(AppCommand.ChangeStatus command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.ChangeStatus(command));
        }


        public async Task<AppDto> HandleAsync(Command.Find<AppDto> command)
        {
            var dto = this._storage.AsQueryable<AppDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
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

        public async Task HandleAsync(Command.Delete<App> command)
        {
            await this.DeleteAsync<App>(command.Id);
        }

        private Task<App> FindAppAsync(App app)
        {
            var oldApp = this._storage.AsQueryable<App>()
                                   .FirstOrDefault(s => s.Name == app.Name &&
                                                        s.ClusterId == app.ClusterId &&
                                                        s.Id != app.Id);
            return Task.FromResult(oldApp);
        }
    }
}