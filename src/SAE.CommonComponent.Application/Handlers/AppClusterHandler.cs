using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using AppClusterCommand = SAE.CommonComponent.Application.Commands.AppClusterCommand;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AppClusterHandler : AbstractHandler,
                              ICommandHandler<AppClusterCommand.Create, string>,
                              ICommandHandler<AppClusterCommand.Change>,
                              ICommandHandler<Command.Delete<AppCluster>>,
                              ICommandHandler<AppClusterCommand.ChangeStatus>,
                              ICommandHandler<AppClusterCommand.Query, IPagedList<AppClusterDto>>,
                              ICommandHandler<AppClusterCommand.Find, AppClusterDto>,
                              ICommandHandler<AppClusterCommand.List, IEnumerable<AppClusterDto>>
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
            await appCluster.ExistAsync(this.FindClusterAsync);
            appCluster.Change(command);
            await this._documentStore.SaveAsync(appCluster);
        }

        public async Task<string> HandleAsync(AppClusterCommand.Create command)
        {
            if (!command.Id.IsNullOrWhiteSpace())
            {
                var count = this._storage.AsQueryable<AppCluster>().Count();
                if (count > 0)
                {
                    command.Id = string.Empty;
                }
            }

            var appCluster = new AppCluster(command);
            await appCluster.ExistAsync(this.FindClusterAsync);
            await this.AddAsync(appCluster);
            return appCluster.Id;
        }

        public Task HandleAsync(AppClusterCommand.ChangeStatus command)
        {
            return this.UpdateAsync<AppCluster>(command.Id, app => app.ChangeStatus(command));
        }


        public async Task<AppClusterDto> HandleAsync(AppClusterCommand.Find command)
        {
            if (!command.Id.IsNullOrWhiteSpace())
            {
                return this._storage.AsQueryable<AppClusterDto>()
                                    .FirstOrDefault(s => s.Id == command.Id);
            }
            else if (!command.Type.IsNullOrWhiteSpace())
            {
                return this._storage.AsQueryable<AppClusterDto>()
                                    .FirstOrDefault(s => s.Type == command.Type);
            }
            return null;
        }

        public Task<IPagedList<AppClusterDto>> HandleAsync(AppClusterCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppClusterDto>();
            if (!command.Key.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Key) || s.Description.Contains(command.Key));
            }
            if (!command.Type.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Type == command.Type);
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task HandleAsync(Command.Delete<AppCluster> command)
        {
            await this.DeleteAsync<AppCluster>(command.Id);
        }

        public async Task<IEnumerable<AppClusterDto>> HandleAsync(AppClusterCommand.List command)
        {
            var query = this._storage.AsQueryable<AppClusterDto>();
            if (!command.Type.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Type == command.Type);
            }

            return query.ToArray();
        }

        private Task<AppCluster> FindClusterAsync(AppCluster cluster)
        {
            AppCluster oldAppCluster;
            if (cluster.Type.IsNullOrWhiteSpace())
            {
                oldAppCluster = this._storage.AsQueryable<AppCluster>()
                                             .FirstOrDefault(s => s.Name == cluster.Name &&
                                                                  s.Id != cluster.Id);
            }
            else
            {
                oldAppCluster = this._storage.AsQueryable<AppCluster>()
                                         .FirstOrDefault(s => (s.Name == cluster.Name || s.Type == cluster.Type) &&
                                                         s.Id != cluster.Id);
            }
            return Task.FromResult(oldAppCluster);
        }

    }
}