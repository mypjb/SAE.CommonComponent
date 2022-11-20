using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class ClientRoleHandler : AbstractHandler<ClientRole>,
                                  ICommandHandler<ClientRoleCommand.ReferenceRole>,
                                  ICommandHandler<ClientRoleCommand.DeleteRole>,
                                  ICommandHandler<ClientRoleCommand.QueryClientAuthorizeCode, Dictionary<string, string>>,
                                  ICommandHandler<ClientRoleCommand.Query, IPagedList<RoleDto>>,
                                  ICommandHandler<Command.Find<ClientRoleDto>, IEnumerable<RoleDto>>,
                                  ICommandHandler<Command.BatchDelete<ClientRole>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly IBitmapAuthorization _bitmapAuthorization;
        private readonly ILogging _logging;

        public ClientRoleHandler(IDocumentStore documentStore,
                              IStorage storage,
                              IMediator mediator,
                              IDirector director,
                              IBitmapAuthorization bitmapAuthorization,
                              ILogging<ClientRoleHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
            this._bitmapAuthorization = bitmapAuthorization;
            this._logging = logging;
        }


        public async Task HandleAsync(ClientRoleCommand.ReferenceRole command)
        {
            var clientRoles = command.RoleIds.Select(s => new ClientRole(command.ClientId, s));
            await this._documentStore.SaveAsync(clientRoles);
        }

        public Task HandleAsync(ClientRoleCommand.DeleteRole command)
        {
            var clientRoles = command.RoleIds.Select(s => new ClientRole(command.ClientId, s));
            return this._documentStore.DeleteAsync(clientRoles);
        }


        public async Task<IEnumerable<RoleDto>> HandleAsync(Command.Find<ClientRoleDto> command)
        {
            var clientIds = this._storage.AsQueryable<ClientRoleDto>()
                                       .Where(s => s.ClientId == command.Id)
                                       .Select(s => s.RoleId)
                                       .ToArray();

            var roles = this._storage.AsQueryable<RoleDto>()
                                     .Where(s => clientIds.Contains(s.Id))
                                     .ToArray();


            await this._director.Build<IEnumerable<RoleDto>>(roles);

            return roles;
        }

        public async Task<Dictionary<string, string>> HandleAsync(ClientRoleCommand.QueryClientAuthorizeCode command)
        {
            var roles = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new Command.Find<ClientRoleDto>
            {
                Id = command.ClientId
            });

            var dic = new Dictionary<string, string>();
            foreach (var group in roles.GroupBy(s => s.AppId))
            {
                var endpoints = (await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new AppResourceCommand.List
                {
                    AppId = group.Key
                }));

                var permissionBits = new List<int>();

                foreach (var role in group)
                {
                    foreach (var permission in role.Permissions)
                    {
                        var appResource = endpoints.FirstOrDefault(s => string.Format(Constants.Authorize.PermissionFormat, s.Method, s.Path) == permission.Path);

                        if (appResource == null)
                        {
                            this._logging.Warn($"not find '{permission.Path}' permission index");
                        }
                        else
                        {
                            permissionBits.Add(appResource.Index);
                        }
                    }
                }
                var code = this._bitmapAuthorization.GenerateCode(permissionBits);
                dic.Add(group.Key, code);
            }

            return dic;
        }

        public async Task<IPagedList<RoleDto>> HandleAsync(ClientRoleCommand.Query command)
        {
            if (command.Referenced)
            {
                var paging = PagedList.Build(this._storage.AsQueryable<ClientRole>()
                                   .Where(s => s.ClientId == command.ClientId), command);

                var ids = paging.Select(ur => ur.RoleId).ToArray();

                return PagedList.Build(this._storage.AsQueryable<RoleDto>()
                                                    .Where(s => ids.Contains(s.Id))
                                                    .ToList(), paging);
            }
            else
            {
                var ids = this._storage.AsQueryable<ClientRole>()
                                       .Where(s => s.ClientId == command.ClientId)
                                       .Select(s => s.RoleId)
                                       .ToArray();

                return PagedList.Build(this._storage.AsQueryable<RoleDto>()
                                           .Where(s => !ids.Contains(s.Id)), command);
            }
        }

        public Task HandleAsync(Command.BatchDelete<ClientRole> command)
        {
            return this._documentStore.DeleteAsync<ClientRole>(command.Ids);
        }
    }
}
