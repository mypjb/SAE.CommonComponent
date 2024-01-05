using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public class UserRoleHandler : AbstractHandler<UserRole>,
                                  ICommandHandler<UserRoleCommand.ReferenceRole>,
                                  ICommandHandler<UserRoleCommand.DeleteRole>,
                                  ICommandHandler<UserRoleCommand.QueryUserAuthorizeCode, AuthorizeCodeDto>,
                                  ICommandHandler<UserRoleCommand.Query, IPagedList<RoleDto>>,
                                  ICommandHandler<UserRoleCommand.List, IEnumerable<RoleDto>>,
                                  ICommandHandler<Command.BatchDelete<UserRole>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly ILogging _logging;

        public UserRoleHandler(IDocumentStore documentStore,
                              IStorage storage,
                              IMediator mediator,
                              IDirector director,
                              ILogging<UserRoleHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
            this._logging = logging;
        }


        public async Task HandleAsync(UserRoleCommand.ReferenceRole command)
        {
            var userRoles = command.RoleIds.Select(s => new UserRole(command.UserId, s));
            await this._documentStore.SaveAsync(userRoles);
        }

        public Task HandleAsync(UserRoleCommand.DeleteRole command)
        {
            var userRoles = command.RoleIds.Select(s => new UserRole(command.UserId, s));
            return this._documentStore.DeleteAsync(userRoles);
        }


        public async Task<IEnumerable<RoleDto>> HandleAsync(UserRoleCommand.List command)
        {
            var query = from role in this._storage.AsQueryable<RoleDto>()
                        join ur in this._storage.AsQueryable<UserRoleDto>()
                        on role.Id equals ur.RoleId
                        where ur.UserId == command.UserId
                        select role;

            var roles = query.ToArray();

            await this._director.Build<IEnumerable<RoleDto>>(roles);

            return roles;
        }

        public async Task<AuthorizeCodeDto> HandleAsync(UserRoleCommand.QueryUserAuthorizeCode command)
        {
            var roles = await this._mediator.SendAsync<IEnumerable<RoleDto>>(new UserRoleCommand.List
            {
                UserId = command.UserId
            });

            var appIds = await this._mediator.SendAsync<IEnumerable<string>>(new SuperAdminCommand.List
            {
                TargetId = command.UserId
            });

            var dic = new Dictionary<string, string>();
            foreach (var group in roles.GroupBy(s => s.AppId))
            {
                if (appIds.Contains(group.Key))
                {
                    this._logging.Info($"用户'{command.UserId}'具有系统'{group.Key}'超管权限，忽略角色引用!");
                    continue;
                }
                var indexs = group.Select(s => s.Index).ToArray();
                // var code = this._bitmapAuthorization.GenerateCode(indexs);
                // if (code.IsNullOrWhiteSpace())
                // {
                //     this._logging.Warn($"系统({group.Key})下的用户({command.UserId})分配的角色，尚未分配任何权限，或权限索引皆为0。");
                //     continue;
                // }
                // dic.Add(group.Key, code);
            }

            return new AuthorizeCodeDto
            {
                Codes = dic,
                SuperAdmins = appIds
            };
        }

        public async Task<IPagedList<RoleDto>> HandleAsync(UserRoleCommand.Query command)
        {
            var query = from role in this._storage.AsQueryable<RoleDto>()
                        join ur in this._storage.AsQueryable<UserRoleDto>()
                        on role.Id equals ur.RoleId
                        where ur.UserId == command.UserId
                        select role;

            return PagedList.Build(query, command);
        }

        public Task HandleAsync(Command.BatchDelete<UserRole> command)
        {
            return this._documentStore.DeleteAsync<UserRole>(command.Ids);
        }
    }
}
