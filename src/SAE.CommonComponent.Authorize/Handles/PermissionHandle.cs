using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.AspNetCore.Routing;
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
                                    ICommandHandler<PermissionCommand.Create, string>,
                                    ICommandHandler<PermissionCommand.Change>,
                                    ICommandHandler<PermissionCommand.ChangeStatus>,
                                    ICommandHandler<Command.BatchDelete<Permission>>,
                                    ICommandHandler<Command.Find<PermissionDto>, PermissionDto>,
                                    ICommandHandler<PermissionCommand.Query, IPagedList<PermissionDto>>,
                                    ICommandHandler<Command.List<PermissionDto>, IEnumerable<PermissionDto>>,
                                    ICommandHandler<IEnumerable<PermissionCommand.Create>, IEnumerable<BitmapEndpoint>>

    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;

        public PermissionHandle(IDocumentStore documentStore, IStorage storage, IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
        }

        public async Task<string> Handle(PermissionCommand.Create command)
        {
            var Permission = new Permission(command);
            await Permission.NameExist(this.FindPermission);
            await this.Add(Permission);
            return Permission.Id;
        }

        public async Task Handle(PermissionCommand.Change command)
        {
            await this.Update(command.Id, async Permission =>
             {
                 Permission.Change(command);
                 await Permission.NameExist(this.FindPermission);
             });
        }

        public async Task Handle(PermissionCommand.ChangeStatus command)
        {
            await this.Update(command.Id, Permission =>
            {
                Permission.ChangeStatus(command);
            });
        }

        public async Task Handle(Command.BatchDelete<Permission> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                var Permission = await this._documentStore.FindAsync<Permission>(id);
                Permission.Remove();
                await this._documentStore.SaveAsync(Permission);
            });
        }

        public Task<IPagedList<PermissionDto>> Handle(PermissionCommand.Query command)
        {
            var query = this.GetStorage();

            if (command.Name.IsNotNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name));

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task<IEnumerable<PermissionDto>> Handle(Command.List<PermissionDto> command)
        {
            return this.GetStorage().ToArray();
        }

        public async Task<PermissionDto> Handle(Command.Find<PermissionDto> command)
        {
            var dto = this.GetStorage()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public async Task<IEnumerable<BitmapEndpoint>> Handle(IEnumerable<PermissionCommand.Create> commands)
        {
            var endpoints = new List<BitmapEndpoint>();

            var dtos = (await this._mediator.Send<IEnumerable<PermissionDto>>(new Command.List<PermissionDto>()))
                                .ToList();

            var permissionCreateCommands = commands.Where(c => !dtos.Any(s => s.Flag.Equals(c.Flag, StringComparison.OrdinalIgnoreCase)))
                                                   .ToArray();

            if (permissionCreateCommands.Any())//不存在
            {

                var tasks = permissionCreateCommands.Select(async command =>
                 {
                     var permission = new Permission(command);
                     await permission.NameExist(this.FindPermission);
                     return permission;
                 });

                var permissions = await Task.WhenAll(tasks);

                await this._documentStore.SaveAsync(permissions);

                dtos = (await this._mediator.Send<IEnumerable<PermissionDto>>(new Command.List<PermissionDto>()))
                                    .ToList();
            }

            foreach (var command in commands)
            {
                endpoints.Add(new BitmapEndpoint
                {
                    Name = command.Name,
                    Index = dtos.FindIndex(s => s.Flag.Equals(command.Flag, StringComparison.OrdinalIgnoreCase)),
                    Path = command.Flag,
                });
            }

            return endpoints;
        }

        private IQueryable<PermissionDto> GetStorage()
        {
            return this._storage.AsQueryable<PermissionDto>().Where(s => s.Status != Status.Delete);
        }
        private Task<string> FindPermission(string name)
        {
            var dto = this._storage.AsQueryable<PermissionDto>()
                                   .FirstOrDefault(s => s.Name.Contains(name));
            return Task.FromResult(dto?.Name ?? string.Empty);
        }
    }
}
