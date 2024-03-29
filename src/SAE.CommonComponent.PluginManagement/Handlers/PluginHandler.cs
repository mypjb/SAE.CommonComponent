using SAE.CommonComponent.PluginManagement.Commands;
using SAE.CommonComponent.PluginManagement.Domians;
using SAE.CommonComponent.PluginManagement.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System.Collections.Generic;
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
                                 ICommandHandler<PluginCommand.Query, IPagedList<PluginDto>>,
                                 ICommandHandler<Command.List<PluginDto>, IEnumerable<PluginDto>>

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
            await this.UpdateAsync(command.Id, plugin =>
            {
                plugin.Change(command);
            });
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
            return PagedList.Build(query.OrderByDescending(s => s.Order), command);
        }

        public async Task HandleAsync(Command.Delete<Plugin> command)
        {
            await this.DeleteAsync(command.Id);
        }

        public async Task HandleAsync(PluginCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, plugin =>
            {
                plugin.ChangeStatus(command);
            });
        }

        public async Task HandleAsync(PluginCommand.ChangeEntry command)
        {
            await this.UpdateAsync(command.Id, plugin =>
            {
                plugin.ChangeEntry(command);
            });
        }

        public Task<IEnumerable<PluginDto>> HandleAsync(Command.List<PluginDto> command)
        {
            var plugins = this._storage.AsQueryable<PluginDto>()
                              .OrderByDescending(s => s.Order)
                              .ToList();

            return Task.FromResult(plugins.AsEnumerable());
        }
    }
}
