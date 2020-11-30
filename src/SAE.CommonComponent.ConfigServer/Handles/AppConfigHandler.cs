using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class AppConfigHandler : ICommandHandler<AppCommand.Config, AppConfigDto>
    {
        private readonly IStorage _storage;
        private readonly IDocumentStore _documentStore;

        public AppConfigHandler(IStorage storage, IDocumentStore documentStore)
        {
            this._storage = storage;
            this._documentStore = documentStore;
        }
        public async Task<AppConfigDto> Handle(AppCommand.Config command)
        {
            var app = new AppConfigDto();

            var projectDto = this._storage.AsQueryable<ProjectDto>().FirstOrDefault(s => s.Id == command.Id);

            app.Version = projectDto.Version;

            if (projectDto.Version != command.Version)
            {

                var projectConfigs = this._storage.AsQueryable<ProjectConfigDto>()
                                                  .Where(s => s.ProjectId == command.Id);

                var environment = this._storage.AsQueryable<EnvironmentVariable>()
                                     .FirstOrDefault(e => e.Name == command.Env);

                if (environment != null)
                {
                    var configs = this._storage.AsQueryable<ConfigDto>()
                                               .Where(s => projectConfigs.Any(pc => pc.ConfigId == s.Id) &&
                                                           s.EnvironmentId == environment.Id)
                                               .ToArray();

                    foreach (var projectConfig in projectConfigs.Where(s => configs.Any(c => c.Id == s.ConfigId)))
                    {
                        app.Add(projectConfig, configs.FirstOrDefault(s => s.Id == projectConfig.ConfigId));
                    }
                }
            }

            return app;
        }
    }
}
