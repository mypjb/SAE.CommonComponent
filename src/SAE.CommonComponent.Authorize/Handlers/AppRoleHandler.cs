using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class AppRoleHandler : AbstractHandler<AppRole>,
                                  ICommandHandler<AppRoleCommand.ReferenceRole>,
                                  ICommandHandler<AppRoleCommand.DeleteRole>,
                                  ICommandHandler<AppRoleCommand.QueryAppAuthorizeCode, string>,
                                  ICommandHandler<AppRoleCommand.Query, IPagedList<RoleDto>>,
                                  ICommandHandler<Command.Find<AppRoleDto>, IEnumerable<RoleDto>>,
                                  ICommandHandler<Command.BatchDelete<AppRole>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly IBitmapAuthorization _bitmapAuthorization;

        public AppRoleHandler(IDocumentStore documentStore,
                              IStorage storage,
                              IMediator mediator,
                              IDirector director,
                              IBitmapAuthorization bitmapAuthorization) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
            this._bitmapAuthorization = bitmapAuthorization;
        }


        public async Task HandleAsync(AppRoleCommand.ReferenceRole command)
        {
            var appRoles = command.RoleIds.Select(s => new AppRole(command.AppId, s));
            await this._documentStore.SaveAsync(appRoles);
        }

        public Task HandleAsync(AppRoleCommand.DeleteRole command)
        {
            var appRoles = command.RoleIds.Select(s => new AppRole(command.AppId, s));
            return this._documentStore.DeleteAsync(appRoles);
        }


        public async Task<IEnumerable<RoleDto>> HandleAsync(Command.Find<AppRoleDto> command)
        {
            var roleIds = this._storage.AsQueryable<AppRoleDto>()
                                       .Where(s => s.AppId == command.Id)
                                       .Select(s => s.RoleId)
                                       .ToArray();

            var roles = this._storage.AsQueryable<RoleDto>()
                                     .Where(s => roleIds.Contains(s.Id))
                                     .AsEnumerable();


            await this._director.Build(roles);

            return roles;
        }

        public async Task<string> HandleAsync(AppRoleCommand.QueryAppAuthorizeCode command)
        {
            var roles = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new Command.Find<AppRoleDto>
            {
                Id = command.AppId
            });

            var endpoints = (await this._mediator.SendAsync<IEnumerable<BitmapEndpoint>>(new Command.List<BitmapEndpoint>()))
                                       .ToList();

            var permissionBits = new List<int>();

            foreach (var role in roles)
            {
                foreach (var permission in role.Permissions)
                {
                    var index = endpoints.FindIndex(s => s.Name.Equals(permission.Name, StringComparison.OrdinalIgnoreCase));
                    permissionBits.Add(index);
                }
            }

            var code = this._bitmapAuthorization.GeneratePermissionCode(permissionBits);

            return code;
        }

        public async Task<IPagedList<RoleDto>> HandleAsync(AppRoleCommand.Query command)
        {
            if (command.Referenced)
            {
                var paging = PagedList.Build(this._storage.AsQueryable<AppRole>()
                                   .Where(s => s.AppId == command.AppId), command);

                var ids = paging.Select(ur => ur.RoleId).ToArray();

                return PagedList.Build(this._storage.AsQueryable<RoleDto>()
                                                    .Where(s=> ids.Contains(s.Id))
                                                    .ToList(), paging);

            }
            else
            {
                var ids = this._storage.AsQueryable<AppRole>()
                                       .Where(s => s.AppId == command.AppId)
                                       .Select(s=>s.RoleId)
                                       .ToArray();

                return PagedList.Build(this._storage.AsQueryable<RoleDto>()
                                           .Where(s => !ids.Contains(s.Id)), command);
            }
        }

        public Task HandleAsync(Command.BatchDelete<AppRole> command)
        {
            return this._documentStore.DeleteAsync<AppRole>(command.Ids);
        }
    }
}
