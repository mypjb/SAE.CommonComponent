using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class RoleHandler : AbstractHandler<Role>,
                              ICommandHandler<RoleCommand.Create, string>,
                              ICommandHandler<RoleCommand.SetIndex>,
                              ICommandHandler<RoleCommand.Change>,
                              ICommandHandler<RoleCommand.ChangeStatus>,
                              ICommandHandler<RoleCommand.ChangePermissionCode>,
                              ICommandHandler<RoleCommand.ReferencePermission>,
                              ICommandHandler<RoleCommand.DeletePermission>,
                              ICommandHandler<Command.BatchDelete<Role>>,
                              ICommandHandler<Command.Find<RoleDto>, RoleDto>,
                              ICommandHandler<RoleCommand.Query, IPagedList<RoleDto>>,
                              ICommandHandler<RoleCommand.List, IEnumerable<RoleDto>>,
                              ICommandHandler<RoleCommand.PermissionList, IEnumerable<PermissionDto>>

    {
        private readonly IStorage _storage;
        private readonly IDirector _director;
        private readonly IMediator _mediator;
        private readonly IMessageQueue _messageQueue;

        public RoleHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IDirector director,
                          IMediator mediator,
                          IMessageQueue messageQueue) : base(documentStore)
        {
            this._storage = storage;
            this._director = director;
            this._mediator = mediator;
            this._messageQueue = messageQueue;
        }

        public async Task<string> HandleAsync(RoleCommand.Create command)
        {
            var role = new Role(command);
            await role.NameExist(this.FindRole);
            await this.AddAsync(role);
            // var handler = ServiceFacade.GetService<IHandler<RoleCommand.Create>>();
            await this._messageQueue.PublishAsync(command);
            return role.Id;
        }

        public async Task HandleAsync(RoleCommand.Change command)
        {
            await this.UpdateAsync(command.Id, async role =>
            {
                role.Change(command);
                await role.NameExist(this.FindRole);
            });
        }

        public async Task HandleAsync(RoleCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, role =>
            {
                role.ChangeStatus(command);
            });
        }

        public async Task HandleAsync(Command.BatchDelete<Role> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                var userRoles = this._storage.AsQueryable<UserRole>()
                                             .Where(s => s.RoleId == id);
                await this._mediator.SendAsync(new Command.BatchDelete<UserRole>
                {
                    Ids = userRoles.Select(s => s.Id)
                });

                var appRoles = this._storage.AsQueryable<ClientRole>()
                                             .Where(s => s.RoleId == id);
                await this._mediator.SendAsync(new Command.BatchDelete<ClientRole>
                {
                    Ids = appRoles.Select(s => s.Id)
                });

                await this._storage.DeleteAsync<Role>(id);

            });
        }

        public async Task<IPagedList<RoleDto>> HandleAsync(RoleCommand.Query command)
        {
            var query = this._storage.AsQueryable<RoleDto>();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name));

            var dtos = PagedList.Build(query, command);

            await this._director.Build(dtos.AsEnumerable());

            return dtos;
        }

        public async Task<RoleDto> HandleAsync(Command.Find<RoleDto> command)
        {
            var dto = this._storage.AsQueryable<RoleDto>()
                                   .FirstOrDefault(s => s.Id == command.Id);
            await this._director.Build(dto);
            return dto;
        }

        public async Task HandleAsync(RoleCommand.ReferencePermission command)
        {
            var role = await this.FindAsync(command.Id);

            Assert.Build(role)
                  .IsNotNull();

            role.ReferencePermission(command);

            await this._documentStore.SaveAsync(role);

            var permissionChangeCommand = new RoleCommand.PermissionChange
            {
                Id = command.Id
            };

            await this._messageQueue.PublishAsync(permissionChangeCommand);
        }

        public async Task HandleAsync(RoleCommand.DeletePermission command)
        {
            var role = await this.FindAsync(command.Id);

            Assert.Build(role)
                  .IsNotNull();

            role.DeletePermission(command);

            await this._documentStore.SaveAsync(role);

            var permissionChangeCommand = new RoleCommand.PermissionChange
            {
                Id = command.Id
            };

            await this._messageQueue.PublishAsync(permissionChangeCommand);
        }

        public async Task<IEnumerable<PermissionDto>> HandleAsync(RoleCommand.PermissionList command)
        {
            var roleDtos = this._storage.AsQueryable<RoleDto>()
                                        .FirstOrDefault(s => s.Id == command.Id);
            await this._director.Build(roleDtos);

            return roleDtos?.Permissions ?? Enumerable.Empty<PermissionDto>();
        }

        public async Task<IEnumerable<RoleDto>> HandleAsync(RoleCommand.List command)
        {
            var query = this._storage.AsQueryable<RoleDto>();
            if (!command.AppId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.AppId == command.AppId);
            }
            if (!command.PermissionId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.PermissionIds.Contains(command.PermissionId));
            }
            return query.ToArray();
        }

        public async Task HandleAsync(RoleCommand.SetIndex command)
        {
            var role = await this.FindAsync(command.Id);
            Assert.Build(role)
                  .NotNull("角色不存在，或被删除！")
                  .Then(s => s.Status == Status.Delete)
                  .False("角色不存在，或被删除！");
            role.SetIndex(command);
            await this._documentStore.SaveAsync(role);
        }

        public async Task HandleAsync(RoleCommand.ChangePermissionCode command)
        {
            await this.UpdateAsync(command.Id, role =>
            {
                role.ChangePermissionCode(command);
            });
        }

        private Task<Role> FindRole(Role role)
        {
            var oldRole = this._storage.AsQueryable<Role>()
                                       .FirstOrDefault(s => s.AppId == role.AppId
                                                       && s.Name == role.Name
                                                       && s.Id != role.Id);
            return Task.FromResult(oldRole);
        }
    }
}
