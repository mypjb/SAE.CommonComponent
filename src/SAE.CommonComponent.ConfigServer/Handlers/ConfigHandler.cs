using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handlers
{
    public class ConfigHandler : AbstractHandler<Config>,
                                 ICommandHandler<ConfigCommand.Create, string>,
                                 ICommandHandler<ConfigCommand.Change>,
                                 ICommandHandler<Command.Delete<Config>>,
                                 ICommandHandler<Command.Find<ConfigDto>, ConfigDto>,
                                 ICommandHandler<ConfigCommand.Query, IPagedList<ConfigDto>>
    {
        private readonly IStorage _storage;

        public ConfigHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;

        }

        public Task<ConfigDto> HandleAsync(Command.Find<ConfigDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<ConfigDto>()
                     .FirstOrDefault(s => s.Id == command.Id));
        }

        public Task HandleAsync(Command.Delete<Config> command)
        {
            return this.DeleteAsync(command.Id);
        }

        public async Task<string> HandleAsync(ConfigCommand.Create command)
        {
            var config = await this.AddAsync(new Config(command));
            return config.Id;
        }

        public Task HandleAsync(ConfigCommand.Change command)
        {
            return this.UpdateAsync(command.Id, s => s.Change(command));
        }

        public async Task<IPagedList<ConfigDto>> HandleAsync(ConfigCommand.Query command)
        {
            var query = this._storage.AsQueryable<ConfigDto>()
                                     .Where(s => s.ClusterId == command.ClusterId &&
                                                 s.EnvironmentId == command.EnvironmentId);
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }
    }
}
