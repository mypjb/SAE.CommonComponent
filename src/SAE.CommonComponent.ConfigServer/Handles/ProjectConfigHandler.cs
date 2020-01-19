using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary.Abstract.Mediator;
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
        public ProjectConfigHandler(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task Handle(ProjectRelevanceConfigCommand command)
        {
            var project = await this._documentStore.FindAsync<Project>(command.Id);

            var projectConfigs = project.Relevance(command.ConfigIds
                                                          .Select(s => new Config { Id = s })
                                                          .ToArray());

            await this._documentStore.SaveAsync(projectConfigs);
        }

        public Task Handle(BatchRemoveCommand<ProjectConfig> command)
        {
            return this._documentStore.RemoveAsync<ProjectConfig>(command.Ids);
        }
    }
}
