using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SAE.CommonComponent.Authorize.Handles
{
    public class UserRoleHandle : AbstractHandler<UserRole>,
                                  ICommandHandler<UserRoleCommand.Reference>,
                                  ICommandHandler<UserRoleCommand.DeleteReference>,
                                  ICommandHandler<Command.Find<UserRoleDto>, IEnumerable<UserRoleDto>>,
                                  ICommandHandler<Command.Find<UserRoleDto>, IEnumerable<RoleDto>>,
                                  ICommandHandler<Command.List<BitmapEndpoint>, IEnumerable<BitmapEndpoint>>,
                                  ICommandHandler<UserRoleCommand.QueryUserAuthorizeCode, string>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly IBitmapAuthorization _bitmapAuthorization;

        public UserRoleHandle(IDocumentStore documentStore,
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


        public async Task Handle(UserRoleCommand.Reference command)
        {
            var userRoles = command.Ids.Select(s => new UserRole(command.UserId, s));
            await this._documentStore.SaveAsync(userRoles);
        }

        public Task Handle(UserRoleCommand.DeleteReference command)
        {
            return this._documentStore.DeleteAsync<UserRole>(command.Ids);
        }

        //public async Task<IEnumerable<PermissionDto>> Handle(UserRoleCommand.QueryRolePermission command)
        //{
        //    var role =await this._mediator.Send<Command.Find<RoleDto>, RoleDto>(new Command.Find<RoleDto>
        //    {
        //         Id=command.RoleId
        //    });

        //    return role.Permissions;
        //}

        public async Task<IEnumerable<RoleDto>> Handle(Command.Find<UserRoleDto> command)
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

        public async Task<IEnumerable<BitmapEndpoint>> Handle(Command.List<BitmapEndpoint> command)
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

        public async Task<string> Handle(UserRoleCommand.QueryUserAuthorizeCode command)
        {
            var roles = await this._mediator.Send<IEnumerable<RoleDto>>(new Command.Find<UserRoleDto>
            {
                Id = command.UserId
            });

            var endpoints = (await this._mediator.Send<IEnumerable<BitmapEndpoint>>(new Command.List<BitmapEndpoint>()))
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

        async Task<IEnumerable<UserRoleDto>> ICommandHandler<Command.Find<UserRoleDto>, IEnumerable<UserRoleDto>>.Handle(Command.Find<UserRoleDto> command)
        {
            var userRoles = this._storage.AsQueryable<UserRoleDto>()
                                         .Where(s => s.UserId == command.Id)
                                         .AsEnumerable();

            await this._director.Build(userRoles);

            return userRoles;
        }
    }
}
