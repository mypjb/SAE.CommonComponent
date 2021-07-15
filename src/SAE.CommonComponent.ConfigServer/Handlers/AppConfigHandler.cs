using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
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
        private readonly IMediator _mediator;

        public AppConfigHandler(IStorage storage,
                                IDocumentStore documentStore,
                                IMediator mediator)
        {
            this._storage = storage;
            this._documentStore = documentStore;
            this._mediator = mediator;
        }
        public async Task<AppConfigDto> HandleAsync(AppCommand.Config command)
        {
            var app = new AppConfigDto();

            var envs = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Environment
            });

            var env = envs.FirstOrDefault(s => s.Name == command.Env);
            Assert.Build(env)
                  .NotNull($"env '{command.Env}' not exist!");

            var projectData = this._storage.AsQueryable<ProjectData>()
                                               .FirstOrDefault(s => s.ProjectId == command.Id &&
                                                               s.EnvironmentId == env.Id);
            app.Version = projectData.Version;
            if (projectData != null && projectData.Version != command.Version)
            {
                app.Data = projectData.Data.ToObject<Dictionary<string, object>>();
            }

            return app;
        }
    }
}
