using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
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
        public async Task<AppConfigDto> HandleAsync(AppCommand.Config command)
        {
            var app = new AppConfigDto();

            var environment = this._storage.AsQueryable<EnvironmentVariableDto>()
                                    .FirstOrDefault(e => e.Name == command.Env);

            if (environment != null)
            {
                var projectData = this._storage.AsQueryable<ProjectData>()
                                               .FirstOrDefault(s => s.ProjectId == command.Id &&
                                                               s.EnvironmentId == environment.Id);

                if (projectData != null && projectData.Version != command.Version)
                {
                    app.Version = projectData.Version;
                    app.Data = projectData.Data.ToObject<Dictionary<string, object>>();
                }
            }
            return app;
        }
    }
}
