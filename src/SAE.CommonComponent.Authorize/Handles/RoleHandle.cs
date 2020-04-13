using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
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

namespace SAE.CommonComponent.Authorize.Handles
{
    public class RoleHandle : AbstractHandler<Role>,
                              ICommandHandler<RoleCreateCommand, string>,
                              ICommandHandler<RoleChangeCommand>,
                              ICommandHandler<RoleChangeStatusCommand>,
                              ICommandHandler<BatchRemoveCommand<Role>>,
                              ICommandHandler<string,RoleDto>,
                              ICommandHandler<RoleQueryCommand, IPagedList<RoleDto>>

    {
        private readonly IStorage _storage;
        private readonly IDirector _director;

        public RoleHandle(IDocumentStore documentStore, 
                          IStorage storage,
                          IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._director = director;
        }

        public async Task<string> Handle(RoleCreateCommand command)
        {
            var role = new Role(command);
            await role.NameExist(this.FindRole);
            await this.Add(role);
            return role.Id;
        }

        public async Task Handle(RoleChangeCommand command)
        {
            await this.Update(command.Id,async role =>
            {
                role.Change(command);
                await role.NameExist(this.FindRole);
            });
        }

        public async Task Handle(RoleChangeStatusCommand command)
        {
            await this.Update(command.Id, role =>
            {
                role.ChangeStatus(command);
            });
        }

        public async Task Handle(BatchRemoveCommand<Role> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                var role= await this._documentStore.FindAsync<Role>(id);
                role.Remove();
                await this._documentStore.SaveAsync(role);
            });
        }

        public async Task<IPagedList<RoleDto>> Handle(RoleQueryCommand command)
        {
            var query = this._storage.AsQueryable<RoleDto>()
                                     .Where(s => s.Status > Status.Delete);
            if (command.Name.IsNotNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name));

            var dtos = PagedList.Build(query, command);

            await this._director.Build(dtos.AsEnumerable());

            return dtos;
        }

        public async Task<RoleDto> Handle(string id)
        {
            var dto= this._storage.AsQueryable<RoleDto>()
                                  .FirstOrDefault(s => s.Id == id);
            return dto;
        }

        private Task<string> FindRole(string name)
        {
            var dto = this._storage.AsQueryable<RoleDto>()
                                   .FirstOrDefault(s => s.Name.Contains(name));
            return Task.FromResult(dto.Name ?? string.Empty);
        }
    }
}
