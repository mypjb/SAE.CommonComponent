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
using System.Linq;
using System.Threading.Tasks;
using AppClusterCommand = SAE.CommonComponent.Application.Commands.AppClusterCommand;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AppClusterHandler : AbstractHandler,
                              ICommandHandler<AppClusterCommand.Create, string>,
                              ICommandHandler<AppClusterCommand.Change>,
                              ICommandHandler<Command.Delete<AppCluster>>,
                              ICommandHandler<AppClusterCommand.ChangeStatus>,
                              ICommandHandler<AppClusterCommand.Query, IPagedList<AppClusterDto>>,
                              ICommandHandler<Command.Find<AppClusterDto>, AppClusterDto>                              
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly ILogging<AppHandler> _logging;
        public AppClusterHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IMediator mediator,
                          ILogging<AppHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
        }


        public async Task HandleAsync(AppClusterCommand.Change command)
        {
            var appCluster = await this._documentStore.FindAsync<AppCluster>(command.Id);
            await appCluster.NameExistAsync(this.FindClusterAsync);
            appCluster.Change(command);
            await this._documentStore.SaveAsync(appCluster);
        }

        public async Task<string> HandleAsync(AppClusterCommand.Create command)
        {
            var appCluster = new AppCluster(command);
            await appCluster.NameExistAsync(this.FindClusterAsync);
            await this.AddAsync(appCluster);
            return appCluster.Id;
        }

        public Task HandleAsync(AppClusterCommand.ChangeStatus command)
        {
            return this.UpdateAsync<AppCluster>(command.Id, app => app.ChangeStatus(command));
        }


        public async Task<AppClusterDto> HandleAsync(Command.Find<AppClusterDto> command)
        {
            var dto = this._storage.AsQueryable<AppClusterDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public Task<IPagedList<AppClusterDto>> HandleAsync(AppClusterCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppClusterDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }
       
        public async Task HandleAsync(Command.Delete<AppCluster> command)
        {
            await this.DeleteAsync<AppCluster>(command.Id);
        }

        private Task<AppCluster> FindClusterAsync(AppCluster cluster)
        {
            var oldAppCluster = this._storage.AsQueryable<AppCluster>()
                                   .FirstOrDefault(s => s.Name == cluster.Name
                                                        && s.Id != cluster.Id);
            return Task.FromResult(oldAppCluster);
        }
    }
}