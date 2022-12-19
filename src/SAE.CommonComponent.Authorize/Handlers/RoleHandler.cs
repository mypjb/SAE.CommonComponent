using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
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
                              ICommandHandler<RoleCommand.PermissionList, IEnumerable<PermissionDto>>,
                              ICommandHandler<RoleCommand.BitmapAuthorizationDescriptors, BitmapAuthorizationDescriptorListDto>

    {
        private readonly IStorage _storage;
        private readonly IDirector _director;
        private readonly IMediator _mediator;
        private readonly IMessageQueue _messageQueue;
        private readonly ILogging _logging;

        public RoleHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IDirector director,
                          IMediator mediator,
                          IMessageQueue messageQueue,
                          ILogging<RoleHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._director = director;
            this._mediator = mediator;
            this._messageQueue = messageQueue;
            this._logging = logging;
        }

        public async Task<string> HandleAsync(RoleCommand.Create command)
        {
            var role = new Role(command);
            await role.NameExist(this.FindRole);
            await this.AddAsync(role);
            // var handler = ServiceFacade.GetService<IHandler<RoleCommand.Create>>();
            var roleEvent = role.To<RoleEvent>();
            await this._messageQueue.PublishAsync(roleEvent);
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
            var role = this._storage.AsQueryable<RoleDto>()
                                    .Where(s => s.Id == command.Id)
                                    .FirstOrDefault();

            await this._director.Build<IEnumerable<RoleDto>>(new[] { role });

            return role?.Permissions ?? Enumerable.Empty<PermissionDto>();
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
            if (command.Status != Status.ALL)
            {
                query = query.Where(s => s.Status == command.Status);
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

        public async Task<BitmapAuthorizationDescriptorListDto> HandleAsync(RoleCommand.BitmapAuthorizationDescriptors command)
        {
            var query = this._storage.AsQueryable<RoleDto>();

            Assert.Build(command.AppId.IsNullOrWhiteSpace() && command.ClusterId.IsNullOrWhiteSpace())
                  .False("请提供集群或系统标识");

            if (!command.ClusterId.IsNullOrWhiteSpace())
            {
                var apps = await this._mediator.SendAsync<IEnumerable<AppDto>>(new AppCommand.List
                {
                    ClusterId = command.ClusterId
                });

                var appIds = apps.Where(s => s.Status == Status.Enable)
                                 .Select(s => s.Id)
                                 .ToArray();
                if (appIds.Any())
                {
                    query = query.Where(s => appIds.Contains(s.Id));
                }
                else
                {
                    query = Enumerable.Empty<RoleDto>().AsQueryable();
                }

            }
            else if (!command.AppId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.AppId == command.AppId);
            }

            var appResources = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new AppResourceCommand.List
            {
                Status = Status.Enable,
                AppId = command.AppId,
                ClusterId = command.ClusterId
            });

            query = query.Where(s => s.Status == Status.Enable);

            var roles = query.ToArray();

            var sb = new StringBuilder();

            roles.ForEach(r =>
            {
                if (sb.Length > 0)
                {
                    sb.Append(Constants.DefaultSeparator);
                }
                sb.Append(r.Id)
                  .Append(Constants.DefaultSeparator)
                  .Append(r.PermissionCode);
            });

            var bitmapAuthorizationDescriptorList = new BitmapAuthorizationDescriptorListDto
            {
                Version = sb.ToString().ToMd5()
            };

            var dict = roles.GroupBy(s => s.AppId)
                            .ToDictionary(s => s.Key,
                                s => new Dictionary<string, object>
                                {
                                    {
                                        BitmapAuthorizationDescriptor.Option,
                                        s.Select(r=> new BitmapAuthorizationDescriptor
                                        {
                                            Index=r.Index,
                                            Name=r.Name,
                                            Description=r.Description,
                                            Code=r.PermissionCode
                                        })
                                    }
                                });

            var appResourceDict = appResources.GroupBy(s => s.AppId)
                            .ToDictionary(s => s.Key,
                                s => new Dictionary<string, object>
                                {
                                    {
                                        $"{ConfigurationEndpointOptions.Option}{SAE.CommonLibrary.Configuration.Constants.ConfigSeparator}{nameof(ConfigurationEndpointOptions.BitmapEndpoints)}",
                                        s.Select(ap=> new BitmapEndpoint
                                        {
                                            Index=ap.Index,
                                            Name=ap.Name,
                                            Path=ap.Path,
                                            Method=ap.Method
                                        })
                                    }
                                });

            foreach (var ar in appResourceDict)
            {
                if (dict.TryGetValue(ar.Key, out Dictionary<string, object> dictionary))
                {
                    foreach (var kv in ar.Value)
                    {
                        if (dictionary.ContainsKey(kv.Key))
                        {
                            this._logging.Error($"在角色字典'{ar.Key}':'{kv.Key}'当中存在同名属性,正常流程不应该出现这种状况,请进行排查!");
                        }
                        else
                        {
                            this._logging.Info($"扩充字典属性'{ar.Key}':'{kv.Key}'");
                            dictionary.Add(kv.Key, kv.Value);
                        }
                    }
                }
                else
                {
                    this._logging.Info($"在角色字典当中不存在应用Key'{ar.Key}',扩充根词典");
                    dict[ar.Key] = ar.Value;
                }
            }

            bitmapAuthorizationDescriptorList.Data = dict;

            return bitmapAuthorizationDescriptorList;
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
