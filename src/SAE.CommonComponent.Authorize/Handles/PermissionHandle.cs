using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
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
    public class PermissionHandle : AbstractHandler<Permission>,
                                    ICommandHandler<PermissionCreateCommand, string>,
                                    ICommandHandler<PermissionChangeCommand>,
                                    ICommandHandler<PermissionChangeStatusCommand>,
                                    ICommandHandler<BatchRemoveCommand<Permission>>,
                                    ICommandHandler<PermissionQueryCommand, IPagedList<PermissionDto>>,
                                    ICommandHandler<PermissionQueryALLCommand,IEnumerable<PermissionDto>>

    {
        private readonly IStorage _storage;

        public PermissionHandle(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task<string> Handle(PermissionCreateCommand command)
        {
            var Permission = new Permission(command);
            await Permission.NameExist(this.FindPermission);
            await this.Add(Permission);
            return Permission.Id;
        }

        public async Task Handle(PermissionChangeCommand command)
        {
            await this.Update(command.Id,async Permission =>
            {
                Permission.Change(command);
                await Permission.NameExist(this.FindPermission);
            });
        }

        public async Task Handle(PermissionChangeStatusCommand command)
        {
            await this.Update(command.Id, Permission =>
            {
                Permission.ChangeStatus(command);
            });
        }

        public async Task Handle(BatchRemoveCommand<Permission> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                var Permission= await this._documentStore.FindAsync<Permission>(id);
                Permission.Remove();
                await this._documentStore.SaveAsync(Permission);
            });
        }

        public Task<IPagedList<PermissionDto>> Handle(PermissionQueryCommand command)
        {
            var query = this._storage.AsQueryable<PermissionDto>()
                                     .Where(s => s.Status > Status.Delete);
            if (command.Name.IsNotNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name));

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task<IEnumerable<PermissionDto>> Handle(PermissionQueryALLCommand command)
        {
            return this._storage.AsQueryable<PermissionDto>()
                                .ToArray();
        }

        private Task<string> FindPermission(string name)
        {
            var dto = this._storage.AsQueryable<PermissionDto>()
                                   .FirstOrDefault(s => s.Name.Contains(name));
            return Task.FromResult(dto.Name ?? string.Empty);
        }
    }
}
