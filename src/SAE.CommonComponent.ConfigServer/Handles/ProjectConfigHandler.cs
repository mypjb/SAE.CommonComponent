using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class ProjectConfigHandler : AbstractHandler<ProjectConfig>, 
                                        ICommandHandler<ProjectRelevanceConfigCommand>,
                                        ICommandHandler<BatchRemoveCommand<ProjectConfig>>
    {
        public ProjectConfigHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore, storage)
        {
        }

        public async Task Handle(ProjectRelevanceConfigCommand command)
        {
            var project = await this._documentStore.FindAsync<Project>(command.Id);

            var projectConfigs = project.Relevance(this._storage.AsQueryable<Config>()
                                                                .Where(s=>command.ConfigIds.Contains(s.Id))
                                                                .ToArray());

            await this._documentStore.SaveAsync(projectConfigs);
        }

        public Task Handle(BatchRemoveCommand<ProjectConfig> command)
        {
            return this._documentStore.RemoveAsync<ProjectConfig>(command.Ids);
        }
    }
}
