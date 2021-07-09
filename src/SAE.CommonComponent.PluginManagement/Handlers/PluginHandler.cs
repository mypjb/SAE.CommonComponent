using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Domians;
using SAE.CommonComponent.PluginManagement.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.PluginManagement.Handlers
{
    public class PluginHandler : AbstractHandler<Plugin>,
                                 ICommandHandler<PluginCommand.Create, string>,
                                 ICommandHandler<PluginCommand.Change>,
                                 ICommandHandler<PluginCommand.ChangeStatus>,
                                 ICommandHandler<PluginCommand.ChangeEntry>,
                                 ICommandHandler<Command.Delete<Plugin>>,
                                 ICommandHandler<Command.Find<PluginDto>, PluginDto>,
                                 ICommandHandler<PluginCommand.Query, IPagedList<PluginDto>>

    {
        private readonly IStorage _storage;
        private readonly IMediator mediator;

        public PluginHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this.mediator = mediator;
        }

        public async Task<string> HandleAsync(PluginCommand.Create command)
        {
            var plugin = new Plugin(command);
            var old = await this._documentStore.FindAsync<Plugin>(plugin.Id);
            if (old == null)
            {
                await this.AddAsync(plugin);
                return plugin.Id;
            }
            else
            {
                var changeCommand = command.To<PluginCommand.Change>();
                changeCommand.Id = old.Id;
                await this.mediator.SendAsync(changeCommand);
                return old.Id;
            }

        }

        public async Task HandleAsync(PluginCommand.Change command)
        {
            var plugin = await this._documentStore.FindAsync<Plugin>(command.Id);
            plugin.Change(command);
            await this._documentStore.SaveAsync(plugin);
        }

        public async Task<PluginDto> HandleAsync(Command.Find<PluginDto> command)
        {
            var first = this._storage.AsQueryable<PluginDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            return first;
        }

        public async Task<IPagedList<PluginDto>> HandleAsync(PluginCommand.Query command)
        {
            var query = this._storage.AsQueryable<PluginDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }

        public async Task HandleAsync(Command.Delete<Plugin> command)
        {
            await this._documentStore.DeleteAsync<Plugin>(command.Id);
        }

        public async Task HandleAsync(PluginCommand.ChangeStatus command)
        {
            var plugin = await this._documentStore.FindAsync<Plugin>(command.Id);
            plugin.ChangeStatus(command);
            await this._documentStore.SaveAsync(plugin);
        }

        public async Task HandleAsync(PluginCommand.ChangeEntry command)
        {
            var plugin = await this._documentStore.FindAsync<Plugin>(command.Id);
            plugin.ChangeEntry(command);
            await this._documentStore.SaveAsync(plugin);
        }
    }
}
