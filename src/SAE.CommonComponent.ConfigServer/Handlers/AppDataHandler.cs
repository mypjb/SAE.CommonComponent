using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Scope.AspNetCore;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class AppDataHandler : ICommandHandler<AppDataCommand.Find, AppDataDto>
    {
        private readonly IStorage _storage;
        private readonly IDocumentStore _documentStore;
        private readonly IMediator _mediator;

        public AppDataHandler(IStorage storage,
                                IDocumentStore documentStore,
                                IMediator mediator)
        {
            this._storage = storage;
            this._documentStore = documentStore;
            this._mediator = mediator;
        }
        public async Task<AppDataDto> HandleAsync(AppDataCommand.Find command)
        {
            var app = new AppDataDto();

            var envs = await this._mediator.SendAsync<IEnumerable<DictItemDto>>(new DictCommand.Tree
            {
                Type=nameof(DictType.Environment)
            });

            var env = envs.FirstOrDefault(s => s.Name == command.Env);

            Assert.Build(env)
                  .NotNull($"此环境'{command.Env}'不存在");

            var configData = new Dictionary<string, object>();

            if (!command.ClusterId.IsNullOrWhiteSpace())
            {
                var appCluster = await this._mediator.SendAsync<AppClusterDto>(new AppClusterCommand.Find
                {
                    Id = command.ClusterId
                });

                var apps = await this._mediator.SendAsync<IEnumerable<AppDto>>(new AppCommand.List
                {
                    ClusterId = command.ClusterId
                });

                Assert.Build(appCluster)
                      .NotNull($"集群'{command.ClusterId}'不存在！");

                var appIds = apps.Select(s => s.Id).ToArray();

                var appDatas = this._storage.AsQueryable<AppConfigData>()
                                                  .Where(s => appIds.Contains(s.AppId) &&
                                                         s.EnvironmentId == env.Id)
                                                  .ToArray();

                var scope = new Dictionary<string, object>();

                foreach (var appData in appDatas)
                {
                    scope[appData.AppId] = (command.Private ?
                                            appData.Data :
                                            appData.PublicData)
                                                   .ToObject<Dictionary<string, object>>();
                }

                var mapping = apps.ToDictionary(s => s.Domain, s => s.Id);

                configData[SAE.CommonLibrary.Constants.Scope] = scope;

                configData[$"{MultiTenantOptions.Option}:{nameof(MultiTenantOptions.Mapper)}"] = mapping;

                if (appCluster.Setting != null)
                {
                    configData[MultiTenantOptions.Option] = appCluster.Setting;
                }

                if (appDatas.Any())
                {
                    app.Version = appDatas.Sum(s => s.Version);
                }

            }
            else
            {
                var appData = this._storage.AsQueryable<AppConfigData>()
                                           .FirstOrDefault(s => s.AppId == command.AppId &&
                                                           s.EnvironmentId == env.Id);

                Assert.Build(appData)
                          .NotNull($"系统'{command.AppId}'不存在!");

                app.Version = appData.Version;
                
                if (appData != null && appData.Version != command.Version)
                {
                    configData = (command.Private ? appData.Data :
                                                  appData.PublicData)
                                                             .ToObject<Dictionary<string, object>>();
                }
            }

            app.Data = configData;

            return app;
        }
    }
}
