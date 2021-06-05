using System.Globalization;
using System.Runtime.CompilerServices;
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
                                 ICommandHandler<MenuCommand.Create, string>,
                                 ICommandHandler<MenuCommand.Change>,
                                 ICommandHandler<Command.BatchDelete<Menu>>,
                                 ICommandHandler<Command.Find<MenuDto>, MenuDto>,
                                 //  ICommandHandler<MenuQueryCommand, IPagedList<MenuDto>>,
                                 ICommandHandler<Command.List<MenuItemDto>, IEnumerable<MenuItemDto>>
    {
        private readonly IStorage _storage;

        public ConfigHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task<string> HandleAsync(MenuCommand.Create command)
        {
            var menu = new Menu(command);
            await menu.ParentExist(this._documentStore.FindAsync<Menu>);
            await menu.NotExist(this.MenuIsExist);
            await this.AddAsync(menu);
            return menu.Id;
        }

        public async Task HandleAsync(MenuCommand.Change command)
        {
            var menu = await this._documentStore.FindAsync<Menu>(command.Id);
            await menu.Change(command, this._documentStore.FindAsync<Menu>, this.MenuIsExist);
            await this._documentStore.SaveAsync(menu);
        }

        public async Task<MenuDto> HandleAsync(Command.Find<MenuDto> command)
        {
            var first = this._storage.AsQueryable<MenuDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            return first;
        }

        // public async Task<IPagedList<MenuDto>> HandleAsync(MenuQueryCommand command)
        // {
        //     var query = this._storage.AsQueryable<MenuDto>();
        //     if (command.Name.IsNotNullOrWhiteSpace())
        //     {
        //         query = query.Where(s => s.Name.Contains(command.Name));
        //     }
        //     return PagedList.Build(query, command);
        // }

        public async Task<IEnumerable<MenuItemDto>> HandleAsync(Command.List<MenuItemDto> command)
        {
            var menus = this._storage.AsQueryable<MenuDto>()
                                     .Select(s => new MenuItemDto
                                     {
                                         Id = s.Id,
                                         Name = s.Name,
                                         Path = s.Path,
                                         ParentId = s.ParentId
                                     }).ToArray();
            var rootMenus = menus.Where(s => s.ParentId == Menu.DefaultId).ToArray();

            await rootMenus.ForEachAsync(async menu => await this.Permutation(menu, menus));

            return rootMenus;
        }

        private async Task Permutation(MenuItemDto menu, IEnumerable<MenuItemDto> menuItems)
        {

            var items = menuItems.Where(s => s.ParentId == menu.Id).ToArray();
            menu.Items = items;
            await items.ForEachAsync(async item => await this.Permutation(item, menuItems));
        }

        private async Task<bool> MenuIsExist(Menu menu)
        {
            return this._storage.AsQueryable<MenuDto>()
                         .Count(s => s.ParentId == menu.ParentId &&
                                s.Id != menu.Id &&
                               (s.Name == menu.Name ||
                                s.Path == menu.Path)) > 0;
        }

        public Task HandleAsync(Command.BatchDelete<Menu> command)
        {
            return this._documentStore.DeleteAsync<Menu>(command.Ids);
        }
    }
}
