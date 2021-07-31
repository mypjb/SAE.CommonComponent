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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class RoleHandler : AbstractHandler<Role>,
                              ICommandHandler<RoleCommand.Create, string>,
                              ICommandHandler<RoleCommand.Change>,
                              ICommandHandler<RoleCommand.ChangeStatus>,
                              ICommandHandler<RoleCommand.ReferencePermission>,
                              ICommandHandler<RoleCommand.DeletePermission>,
                              ICommandHandler<Command.BatchDelete<Role>>,
                              ICommandHandler<Command.Find<RoleDto>,RoleDto>,
                              ICommandHandler<RoleCommand.Query, IPagedList<RoleDto>>,
                              ICommandHandler<RoleCommand.PermissionQuery, IPagedList<PermissionDto>>,
                              ICommandHandler<RoleCommand.ReferenceMenu>,
                              ICommandHandler<RoleCommand.DeleteMenu>

    {
        private readonly IStorage _storage;
        private readonly IDirector _director;
        private readonly IMediator _mediator;

        public RoleHandler(IDocumentStore documentStore, 
                          IStorage storage,
                          IDirector director,
                          IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this._director = director;
            this._mediator = mediator;
        }

        public async Task<string> HandleAsync(RoleCommand.Create command)
        {
            var role = new Role(command);
            await role.NameExist(this.FindRole);
            await this.AddAsync(role);
            return role.Id;
        }

        public async Task HandleAsync(RoleCommand.Change command)
        {
            await this.UpdateAsync(command.Id,async role =>
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
                                             .Where(s => s.RoleId==id);
                await this._mediator.SendAsync(new Command.BatchDelete<UserRole>
                {
                    Ids = userRoles.Select(s => s.Id)
                });

                var appRoles = this._storage.AsQueryable<AppRole>()
                                             .Where(s => s.RoleId == id);
                await this._mediator.SendAsync(new Command.BatchDelete<AppRole>
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
            var dto= this._storage.AsQueryable<RoleDto>()
                                  .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public async Task HandleAsync(RoleCommand.ReferencePermission command)
        {
            var role =await this.FindAsync(command.Id);

            Assert.Build(role)
                  .IsNotNull();

            role.ReferencePermission(command);

            await this._documentStore.SaveAsync(role);
        }

        public async Task HandleAsync(RoleCommand.DeletePermission command)
        {
            var role = await this.FindAsync(command.Id);

            Assert.Build(role)
                  .IsNotNull();

            role.DeletePermission(command);

            await this._documentStore.SaveAsync(role);
        }

        public async Task<IPagedList<PermissionDto>> HandleAsync(RoleCommand.PermissionQuery command)
        {
            var role = await this._documentStore.FindAsync<Role>(command.Id);

            if (command.Referenced)
            {
                var pIds = PagedList.Build(role.PermissionIds.AsQueryable(), command);
                var permissionDtos = await this._mediator.SendAsync<IEnumerable<PermissionDto>>(new PermissionCommand.Finds
                {
                    Ids = pIds.ToArray()
                });

                return PagedList.Build(permissionDtos, pIds);
            }
            else
            {
                return await this._mediator.SendAsync<IPagedList<PermissionDto>>(new PermissionCommand.Query
                {
                    IgnoreIds = role.PermissionIds
                });
            }
        }

        public async Task HandleAsync(RoleCommand.ReferenceMenu command)
        {                                          
            var role = await this._documentStore.FindAsync<Role>(command.Id);
            role.ReferenceMenu(command);
            await this._documentStore.SaveAsync(role);
        }

        public async Task HandleAsync(RoleCommand.DeleteMenu command)
        {
            var role = await this._documentStore.FindAsync<Role>(command.Id);
            role.DeleteMenu(command);
            await this._documentStore.SaveAsync(role);
        }

        

        private Task<Role> FindRole(string name)
        {
            var role = this._storage.AsQueryable<Role>()
                                   .FirstOrDefault(s => s.Name.Contains(name));
            return Task.FromResult(role);
        }
    }
}
