using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using SAE.CommonComponent.Routing.Domains;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class ConfigHandler : AbstractHandler<Menu>,
                                 ICommandHandler<MenuCreateCommand, string>,
                                 ICommandHandler<MenuChangeCommand>,
                                 ICommandHandler<RemoveCommand<Menu>>,
                                 ICommandHandler<string, MenuDto>,
                                 ICommandHandler<MenuQueryCommand, IPagedList<MenuDto>>
    {
        private readonly IStorage _storage;

        public ConfigHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;

        }

        public async Task<string> Handle(MenuCreateCommand command)
        {
            var menu = new Menu(command);
            await menu.ParentExist(this._documentStore.FindAsync<Menu>);
            await menu.NotExist(this.MenuIsExist);
            await this.Add(menu);
            return menu.Id;
        }

        public async Task Handle(MenuChangeCommand command)
        {
            var menu = await this._documentStore.FindAsync<Menu>(command.Id);
            await menu.Change(command, this._documentStore.FindAsync<Menu>, this.MenuIsExist);
        }

        public Task Handle(RemoveCommand<Menu> command)
        {
            return this.Remove(command.Id);
        }

        public async Task<MenuDto> Handle(string command)
        {
            var first = this._storage.AsQueryable<MenuDto>()
                            .FirstOrDefault(s => s.Id == command);
            return first;
        }

        public async Task<IPagedList<MenuDto>> Handle(MenuQueryCommand command)
        {
            var query = this._storage.AsQueryable<MenuDto>();
            if (command.Name.IsNotNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }

        private async Task<bool> MenuIsExist(Menu menu)
        {
            return this._storage.AsQueryable<MenuDto>()
                         .Count(s => s.ParentId == menu.ParentId &&
                                s.Name == menu.Name &&
                                s.Path == menu.Path) == 0;
        }
    }
}
