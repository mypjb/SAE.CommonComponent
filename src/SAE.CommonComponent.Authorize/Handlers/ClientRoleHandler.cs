using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class ClientRoleHandler : AbstractHandler<ClientRole>,
                                     ICommandHandler<ClientRoleCommand.ReferenceRole>,
                                     ICommandHandler<ClientRoleCommand.DeleteRole>,
                                     ICommandHandler<ClientRoleCommand.QueryClientAuthorizeCode, Dictionary<string, string>>,
                                     ICommandHandler<ClientRoleCommand.Query, IPagedList<RoleDto>>,
                                     ICommandHandler<ClientRoleCommand.List, IEnumerable<RoleDto>>,
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


        public async Task<IEnumerable<RoleDto>> HandleAsync(ClientRoleCommand.List command)
        {

            var query = from role in this._storage.AsQueryable<RoleDto>()
                        join cr in this._storage.AsQueryable<ClientRoleDto>()
                        on role.Id equals cr.RoleId
                        where cr.ClientId == command.ClientId
                        select role;

            var roles = query.ToArray();

            await this._director.Build<IEnumerable<RoleDto>>(roles);

            return roles;
        }

        public async Task<Dictionary<string, string>> HandleAsync(ClientRoleCommand.QueryClientAuthorizeCode command)
        {
            var roles = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new ClientRoleCommand.List
            {
                ClientId = command.ClientId
            });

            var dic = new Dictionary<string, string>();
            foreach (var group in roles.GroupBy(s => s.AppId))
            {
                var indexs = group.Select(s => s.Index).ToArray();

                var code = this._bitmapAuthorization.GenerateCode(indexs);

                if (code.IsNullOrWhiteSpace())
                {
                    this._logging.Warn($"系统({group.Key})下的客户端凭证({command.ClientId})分配的角色，尚未分配任何权限，或权限索引皆为0。");
                    continue;
                }
                dic.Add(group.Key, code);
            }

            return dic;
        }

        public async Task<IPagedList<RoleDto>> HandleAsync(ClientRoleCommand.Query command)
        {
            var query = from role in this._storage.AsQueryable<RoleDto>()
                        join cr in this._storage.AsQueryable<ClientRoleDto>()
                        on role.Id equals cr.RoleId
                        where cr.ClientId == command.ClientId
                        select role;

            return PagedList.Build(query, command);
        }

        public Task HandleAsync(Command.BatchDelete<ClientRole> command)
        {
            return this._documentStore.DeleteAsync<ClientRole>(command.Ids);
        }
    }
}
