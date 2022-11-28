using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.AspNetCore.Routing;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class PermissionHandler : AbstractHandler<Permission>,
                                    ICommandHandler<PermissionCommand.Create, string>,
                                    ICommandHandler<PermissionCommand.Change>,
                                    ICommandHandler<PermissionCommand.ChangeStatus>,
                                    ICommandHandler<Command.BatchDelete<Permission>>,
                                    ICommandHandler<Command.Find<PermissionDto>, PermissionDto>,
                                    ICommandHandler<PermissionCommand.Query, IPagedList<PermissionDto>>,
                                    ICommandHandler<PermissionCommand.List, IEnumerable<PermissionDto>>,
                                    ICommandHandler<PermissionCommand.Finds, IEnumerable<PermissionDto>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IMessageQueue _messageQueue;

        public PermissionHandler(IDocumentStore documentStore,
                                 IStorage storage,
                                 IMediator mediator,
                                 IMessageQueue messageQueue) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._messageQueue = messageQueue;
        }

        public async Task<string> HandleAsync(PermissionCommand.Create command)
        {
            var permission = new Permission();
            var resourceCommand = permission.Create(command);
            await permission.NameExist(this.FindPermission);
            await this.AddAsync(permission);
            if (resourceCommand != null)
            {
                await this._messageQueue.PublishAsync(resourceCommand);
            }
            return permission.Id;
        }

        public async Task HandleAsync(PermissionCommand.Change command)
        {
            PermissionCommand.AppResource resourceCommand = null;
            await this.UpdateAsync(command.Id, async permission =>
             {
                 resourceCommand = permission.Change(command);
                 await permission.NameExist(this.FindPermission);
             });
            if (resourceCommand != null)
            {
                await this._messageQueue.PublishAsync(resourceCommand);
            }
        }

        public async Task HandleAsync(PermissionCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, permission =>
            {
                permission.ChangeStatus(command);
            });

            await this._messageQueue.PublishAsync(command);
        }

        public async Task HandleAsync(Command.BatchDelete<Permission> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                var roles = this._storage.AsQueryable<RoleDto>()
                         .Where(s => s.PermissionIds.Contains(id));
                if (roles.Any())
                {
                    var dto = this._storage.AsQueryable<PermissionDto>().First(s => s.Id.Equals(id));
                    throw new SAE.CommonLibrary.SAEException($"permission:'{dto.Name}'Being quoted by {roles.Select(r => r.Name).Aggregate((a, b) => $"{a},{b}")}");
                }
                await this._storage.DeleteAsync<Permission>(id);
            });

            await this._messageQueue.PublishAsync(command);

        }

        public Task<IPagedList<PermissionDto>> HandleAsync(PermissionCommand.Query command)
        {
            var query = this.GetStorage();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name));

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task<IEnumerable<PermissionDto>> HandleAsync(PermissionCommand.List command)
        {
            var query = this.GetStorage();
            if (!command.AppId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.AppId == command.AppId);
            }
            return query.ToArray();
        }

        public async Task<PermissionDto> HandleAsync(Command.Find<PermissionDto> command)
        {
            var dto = this.GetStorage()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }


        private IQueryable<PermissionDto> GetStorage()
        {
            return this._storage.AsQueryable<PermissionDto>();
        }
        private Task<Permission> FindPermission(Permission permission)
        {
            var oldPermission = this._storage.AsQueryable<Permission>()
                                   .FirstOrDefault(s => s.AppId == permission.AppId
                                                        && s.Name == permission.Name
                                                        && s.Id != permission.Id);
            return Task.FromResult(oldPermission);
        }

        public async Task<IEnumerable<PermissionDto>> HandleAsync(PermissionCommand.Finds command)
        {
            return this._storage.AsQueryable<PermissionDto>()
                       .Where(s => command.Ids.Contains(s.Id))
                       .ToList();
        }
    }
}
