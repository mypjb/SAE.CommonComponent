using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AppHandler : AbstractHandler,
                              ICommandHandler<AppCommand.Create, string>,
                              ICommandHandler<AppCommand.Change>,
                              ICommandHandler<AppCommand.ChangeStatus>,
                              ICommandHandler<AppCommand.DeleteScope>,
                              ICommandHandler<AppCommand.ReferenceScope>,
                              ICommandHandler<AppCommand.RefreshSecret>,
                              ICommandHandler<AppCommand.Query, IPagedList<AppDto>>,
                              ICommandHandler<Command.Find<AppDto>, AppDto>,
                              ICommandHandler<AppCommand.ReferenceProject>,
                              ICommandHandler<AppCommand.DeleteProject>,
                              ICommandHandler<AppCommand.ProjectQuery, IPagedList<ProjectDto>>
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

        public Task HandleAsync(AppCommand.DeleteScope command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.DeleteScope(command));
        }

        public Task HandleAsync(AppCommand.ReferenceScope command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.ReferenceScope(command));
        }

        public Task HandleAsync(AppCommand.RefreshSecret command)
        {
            return this.UpdateAsync<App>(command.Id, app => app.RefreshSecret());
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

        public async Task<IPagedList<ProjectDto>> HandleAsync(AppCommand.ProjectQuery command)
        {
            var app= await this._documentStore.FindAsync<App>(command.Id);
            Assert.Build(app)
                  .NotNull();
            return await this._mediator.SendAsync<IPagedList<ProjectDto>>(new ProjectCommand.Query
            {
                IgnoreIds = app.ProjectIds,
                Name = app.Name,
                SolutionId=command.SolutionId,
                PageIndex = command.PageIndex,
                PageSize = command.PageSize
            });
        }

        public async Task HandleAsync(AppCommand.DeleteProject command)
        {
            await this.UpdateAsync<App>(command.Id, app => app.DeleteProject(command));
        }

        public async Task HandleAsync(AppCommand.ReferenceProject command)
        {
            await this.UpdateAsync<App>(command.Id, app => app.ReferenceProject(command));
        }

    }
}