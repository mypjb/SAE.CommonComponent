using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.MultiTenant.Commands;
using SAE.CommonComponent.MultiTenant.Domains;
using SAE.CommonComponent.MultiTenant.Dtos;
using SAE.CommonComponent.MultiTenant.Events;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.MultiTenant.Handlers
{
    public class ConfigHandler : AbstractHandler<Tenant>,
                                 ICommandHandler<TenantCommand.Create, string>,
                                 ICommandHandler<TenantCommand.Change>,
                                 ICommandHandler<TenantCommand.ChangeStatus>,
                                 ICommandHandler<Command.Delete<Tenant>>,
                                 ICommandHandler<Command.Find<TenantDto>, TenantDto>,
                                 ICommandHandler<TenantCommand.Query, IPagedList<TenantDto>>,
                                 ICommandHandler<TenantCommand.Tree, IEnumerable<TenantItemDto>>,
                                 ICommandHandler<TenantCommand.List, IEnumerable<TenantDto>>,
                                 ICommandHandler<TenantCommand.App.Create, string>,
                                 ICommandHandler<TenantCommand.App.Query, IPagedList<AppDto>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;

        public ConfigHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator,
            IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
        }

        private async Task PermutationAsync(TenantItemDto tenant, IEnumerable<TenantItemDto> tenantItems)
        {
            var items = tenantItems.Where(s => s.ParentId == tenant.Id).ToArray();
            tenant.Items = items;
            await items.ForEachAsync(async item => await this.PermutationAsync(item, tenantItems));
        }

        private async Task ForEachTreeAsync(IEnumerable<TenantItemDto> items, Func<TenantItemDto, Task> func)
        {
            foreach (var item in items)
            {
                if (item.Items != null && item.Items.Any())
                    await this.ForEachTreeAsync(item.Items, func);

                await func(item);
            }
        }

        private Task<bool> TenantIsExistAsync(Tenant tenant)
        {
            return Task.FromResult(this._storage.AsQueryable<TenantDto>()
                         .Count(s => s.ParentId == tenant.ParentId &&
                                     s.Type == tenant.Type &&
                                     s.Id != tenant.Id &&
                                     s.Name == tenant.Name) > 0);
        }

        public async Task<string> HandleAsync(TenantCommand.Create command)
        {
            var tenant = new Tenant(command);
            await tenant.ParentExist(this.FindAsync);
            await tenant.NotExist(this.TenantIsExistAsync);
            await this.AddAsync(tenant);
            return tenant.Id;
        }

        public async Task HandleAsync(TenantCommand.Change command)
        {
            var tenant = await this.FindAsync(command.Id);
            await tenant.Change(command, this.TenantIsExistAsync);
            await this._documentStore.SaveAsync(tenant);
        }

        public async Task<TenantDto> HandleAsync(Command.Find<TenantDto> command)
        {
            var first = this._storage.AsQueryable<TenantDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            if (first != null)
            {
                await this._director.Build<IEnumerable<TenantDto>>(new[] { first });
            }

            return first;
        }

        public async Task<IPagedList<TenantDto>> HandleAsync(TenantCommand.Query command)
        {
            var query = this._storage.AsQueryable<TenantDto>();

            if (!command.Key.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Key) ||
                                    s.Description.Contains(command.Key) ||
                                    s.Domain.Contains(command.Key));
            }

            if (!command.Type.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Type == command.Type);
            }
            var paging = PagedList.Build(query, command);

            await this._director.Build(paging.AsEnumerable());
            return paging;
        }

        public async Task HandleAsync(Command.Delete<Tenant> command)
        {
            var tenant = await this._documentStore.FindAsync<Tenant>(command.Id);

            Assert.Build(tenant)
                  .NotNull("Tenant not exist");

            var roots = await this._mediator.SendAsync<IEnumerable<TenantItemDto>>(new TenantCommand.Tree
            {
                ParentId = tenant.Id,
                Type = tenant.Type
            });

            await this.ForEachTreeAsync(roots, async d =>
            {
                await this._documentStore.DeleteAsync<Tenant>(d.Id);

            });

            await this._documentStore.DeleteAsync(tenant);
        }

        public async Task<IEnumerable<TenantItemDto>> HandleAsync(TenantCommand.Tree command)
        {

            var query = this._storage.AsQueryable<TenantDto>();

            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Tenant.RootId;
            }

            if (!command.Type.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Type == command.Type);
            }

            var tenants = query.Select(s => new TenantItemDto
            {
                Id = s.Id,
                Name = s.Name,
                ParentId = s.ParentId,
                Description = s.Description,
                CreateTime = s.CreateTime,
                Type = s.Type
            }).ToArray();

            var rootTenants = tenants.Where(s => s.ParentId == command.ParentId).ToArray();

            await rootTenants.ForEachAsync(async t => await this.PermutationAsync(t, tenants));

            return rootTenants;
        }

        public async Task<IEnumerable<TenantDto>> HandleAsync(TenantCommand.List command)
        {
            var query = this._storage.AsQueryable<TenantDto>();
            if (!command.ParentId.IsNullOrWhiteSpace())
            {

                query = query.Where(s => s.ParentId == command.ParentId);
            }

            if (!command.Type.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Type == command.Type);
            }

            return query.ToArray();
        }

        public async Task<string> HandleAsync(TenantCommand.App.Create command)
        {
            var clusterFindCommand = new AppClusterCommand.Find
            {
                Type = command.Type
            };

            var appClusterDto = await this._mediator.SendAsync<AppClusterDto>(clusterFindCommand);

            Assert.Build(appClusterDto)
                  .NotNull("集群尚未创建，请先联系管理员创建集群！");

            var appCreateCommand = new AppCommand.Create
            {
                Name = command.Name,
                Description = command.Description,
                Domain = command.Domain,
                ClusterId = appClusterDto.Id
            };

            var appId = await this._mediator.SendAsync<string>(appCreateCommand);

            var tenantApp = new TenantApp(command.TenantId, appId);

            await this._documentStore.SaveAsync(tenantApp);

            return appId;
        }

        public async Task<IPagedList<AppDto>> HandleAsync(TenantCommand.App.Query command)
        {
            if (command.TenantId.IsNullOrWhiteSpace())
            {
                return PagedList.Build(Enumerable.Empty<AppDto>(), command);
            }

            var query = from app in this._storage.AsQueryable<AppDto>()
                        join tp in this._storage.AsQueryable<TenantApp>()
                        on app.Id equals tp.AppId
                        select app;

            if (!command.Key.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Key) ||
                s.Description.Contains(command.Key) ||
                s.Domain.Contains(command.Key));
            }

            return PagedList.Build(query, command);
        }

        public async Task HandleAsync(TenantCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, app => app.ChangeStatus(command));
        }
    }
}
