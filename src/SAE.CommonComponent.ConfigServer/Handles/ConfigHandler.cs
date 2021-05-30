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

namespace SAE.CommonComponent.ConfigServer.Handles
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

        public Task<ConfigDto> Handle(Command.Find<ConfigDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<ConfigDto>()
                     .FirstOrDefault(s => s.Id == command.Id));
        }

        public Task Handle(Command.Delete<Config> command)
        {
            return this.Remove(command.Id);
        }

        public async Task<string> Handle(ConfigCommand.Create command)
        {
            var existEnv = this._storage.AsQueryable<EnvironmentVariableDto>()
                                     .Any(s => s.Id == command.EnvironmentId);
            Assert.Build(existEnv)
                  .True("environment not exist!");
            var config = await this.Add(new Config(command));
            return config.Id;
        }

        public Task Handle(ConfigCommand.Change command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }

        public async Task<IPagedList<ConfigDto>> Handle(ConfigCommand.Query command)
        {
            var query = this._storage.AsQueryable<ConfigDto>()
                                     .Where(s => s.SolutionId == command.SolutionId);
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }
    }
}
