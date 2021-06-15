using SAE.CommonComponent.Routing.Abstract.Commands;
using SAE.CommonComponent.Routing.Domains;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Routing.Handlers
{
    public class RoleMenuHandler : AbstractHandler<RoleMenu>,
                                   ICommandHandler<RoleMenuCommand.ReferenceMenu>,
                                   ICommandHandler<RoleMenuCommand.DeleteMenu>,
                                   ICommandHandler<RoleMenuCommand.Query, IPagedList<MenuDto>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;

        public RoleMenuHandler(IDocumentStore documentStore,
                               IStorage storage,
                               IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
        }

        public async Task<IPagedList<MenuDto>> HandleAsync(RoleMenuCommand.Query command)
        {
            var query = this._storage.AsQueryable<RoleMenu>()
                                     .Where(s => s.RoleId == command.RoleId);
            var menuQuery = this._storage.AsQueryable<MenuDto>();
            if (command.IgnoreRelevance)
            {
                menuQuery = menuQuery.Where(s => !query.Any(q => q.MenuId == s.Id));
            }
            else
            {
                menuQuery = menuQuery.Where(s => query.Any(q => q.MenuId == s.Id));
            }

            var menuDtos = PagedList.Build(menuQuery, command);

            return menuDtos;
        }

        public async Task HandleAsync(RoleMenuCommand.ReferenceMenu command)
        {
            var roleMenus = command.MenuIds.Select(s => new RoleMenu(command.RoleId, s));
            await this._documentStore.SaveAsync(roleMenus);
        }

        public async Task HandleAsync(RoleMenuCommand.DeleteMenu command)
        {
            var roleMenus = command.MenuIds.Select(s => new RoleMenu(command.RoleId, s));
            await this._documentStore.DeleteAsync(roleMenus);
        }
    }
}
