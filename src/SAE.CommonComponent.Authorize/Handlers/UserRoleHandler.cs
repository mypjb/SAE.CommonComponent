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
    public class UserRoleHandler : AbstractHandler<UserRole>,
                                  ICommandHandler<UserRoleCommand.ReferenceRole>,
                                  ICommandHandler<UserRoleCommand.DeleteRole>,
                                  ICommandHandler<UserRoleCommand.QueryUserAuthorizeCode, string>,
                                  ICommandHandler<UserRoleCommand.Query, IPagedList<RoleDto>>,
                                  ICommandHandler<Command.Find<UserRoleDto>, IEnumerable<RoleDto>>,
                                  ICommandHandler<Command.List<BitmapEndpoint>, IEnumerable<BitmapEndpoint>>,
                                  ICommandHandler<Command.BatchDelete<UserRole>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly IBitmapAuthorization _bitmapAuthorization;

        public UserRoleHandler(IDocumentStore documentStore,
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


        public async Task HandleAsync(UserRoleCommand.ReferenceRole command)
        {
            var userRoles = command.Ids.Select(s => new UserRole(command.UserId, s));
            await this._documentStore.SaveAsync(userRoles);
        }

        public Task HandleAsync(UserRoleCommand.DeleteRole command)
        {
            var userRoles = command.Ids.Select(s => new UserRole(command.UserId, s));
            return this._documentStore.DeleteAsync(userRoles);
        }


        public async Task<IEnumerable<RoleDto>> HandleAsync(Command.Find<UserRoleDto> command)
        {
            var roleIds = this._storage.AsQueryable<UserRoleDto>()
                                       .Where(s => s.UserId == command.Id)
                                       .Select(s => s.RoleId)
                                       .ToArray();

            var roles = this._storage.AsQueryable<RoleDto>()
                                     .Where(s => roleIds.Contains(s.Id))
                                     .AsEnumerable();


            await this._director.Build(roles);

            return roles;
        }

        public async Task<IEnumerable<BitmapEndpoint>> HandleAsync(Command.List<BitmapEndpoint> command)
        {

            var dtos = this._storage.AsQueryable<PermissionDto>()
                                    .OrderBy(s => s.Id)
                                    .ToArray();

            var endpoints = new List<BitmapEndpoint>(dtos.Count());

            for (int i = 0; i < dtos.Length; i++)
            {
                endpoints.Add(new BitmapEndpoint
                {
                    Index = i,
                    Path = dtos[i].Flag,
                    Name = dtos[i].Name
                });
            }

            return endpoints;
        }

        public async Task<string> HandleAsync(UserRoleCommand.QueryUserAuthorizeCode command)
        {
            var roles = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new Command.Find<UserRoleDto>
            {
                Id = command.UserId
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

        public async Task<IPagedList<RoleDto>> HandleAsync(UserRoleCommand.Query command)
        {
            if (command.Referenced)
            {
                var ids = PagedList.Build(this._storage.AsQueryable<UserRole>()
                                   .Where(s => s.UserId == command.UserId), command);
                return PagedList.Build(this._storage.AsQueryable<RoleDto>()
                                                    .Where(s=>ids.Select(ur=>ur.RoleId).Contains(s.Id))
                                                    .ToList(), ids);

            }
            else
            {
                var ids = this._storage.AsQueryable<UserRole>()
                                       .Where(s => s.UserId == command.UserId)
                                       .Select(s=>s.RoleId);
                return PagedList.Build(this._storage.AsQueryable<RoleDto>()
                                           .Where(s => !ids.Contains(s.Id)), command);
            }
        }

        public Task HandleAsync(Command.BatchDelete<UserRole> command)
        {
            return this._documentStore.DeleteAsync<UserRole>(command.Ids);
        }
    }
}
