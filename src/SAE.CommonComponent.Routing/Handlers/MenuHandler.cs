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
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Commands;

namespace SAE.CommonComponent.ConfigServer.Handlers
{
    public class ConfigHandler : AbstractHandler<Menu>,
                                 ICommandHandler<MenuCommand.Create, string>,
                                 ICommandHandler<MenuCommand.Change>,
                                 ICommandHandler<Command.BatchDelete<Menu>>,
                                 ICommandHandler<Command.Find<MenuDto>, MenuDto>,
                                 ICommandHandler<MenuCommand.Query, IPagedList<MenuDto>>,
                                 ICommandHandler<MenuCommand.Tree, IEnumerable<MenuItemDto>>,
                                 ICommandHandler<MenuCommand.ReferencePermission>,
                                 ICommandHandler<MenuCommand.DeletePermission>,
                                 ICommandHandler<MenuCommand.PermissionQuery, IPagedList<PermissionDto>>

    {
        private readonly IStorage _storage;
        private readonly IMediator mediator;

        public ConfigHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this.mediator = mediator;
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

        public async Task<IPagedList<MenuDto>> HandleAsync(MenuCommand.Query command)
        {
            var query = this._storage.AsQueryable<MenuDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }

        public async Task<IEnumerable<MenuItemDto>> HandleAsync(MenuCommand.Tree command)
        {
            var menus = this._storage.AsQueryable<MenuDto>()
                                     .Select(s => new MenuItemDto
                                     {
                                         Id = s.Id,
                                         Name = s.Name,
                                         Path = s.Path,
                                         ParentId = s.ParentId,
                                         Hidden = s.Hidden
                                     }).ToArray();
            var rootMenus = menus.Where(s => s.ParentId == Constants.Menu.RootId).ToArray();

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
                                s.AppId == menu.AppId &&
                                s.Id != menu.Id &&
                               (s.Name == menu.Name)) > 0;
        }

        public Task HandleAsync(Command.BatchDelete<Menu> command)
        {
            return this._documentStore.DeleteAsync<Menu>(command.Ids);
        }

        public async Task HandleAsync(MenuCommand.ReferencePermission command)
        {
            var menu = await this._documentStore.FindAsync<Menu>(command.Id);
            menu.ReferencePermission(command);
            await this._documentStore.SaveAsync(menu);
        }

        public async Task HandleAsync(MenuCommand.DeletePermission command)
        {
            var menu = await this._documentStore.FindAsync<Menu>(command.Id);
            menu.DeletePermission(command);
            await this._documentStore.SaveAsync(menu);
        }

        public async Task<IPagedList<PermissionDto>> HandleAsync(MenuCommand.PermissionQuery command)
        {
            var menu = await this._documentStore.FindAsync<Menu>(command.Id);

            if (command.Referenced)
            {
                var pIds = PagedList.Build(menu.PermissionIds.AsQueryable(), command);
                var permissionDtos = await this.mediator.SendAsync<IEnumerable<PermissionDto>>(new PermissionCommand.Finds
                {
                    Ids = pIds.ToArray()
                });

                return PagedList.Build(permissionDtos, pIds);
            }
            else
            {
                return await this.mediator.SendAsync<IPagedList<PermissionDto>>(new PermissionCommand.Query
                {
                    IgnoreIds = menu.PermissionIds
                });
            }
        }

    }
}
