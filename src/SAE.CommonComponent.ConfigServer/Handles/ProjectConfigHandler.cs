using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
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
                                        ICommandHandler<BatchRemoveCommand<ProjectConfig>>,
                                        ICommandHandler<ProjectConfigQueryCommand, IPagedList<ProjectConfigDto>>,
                                        ICommandHandler<ProjectConfigQueryCommand, IPagedList<ConfigDto>>

    {
        private readonly IStorage _storage;
        public ProjectConfigHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task Handle(ProjectRelevanceConfigCommand command)
        {
            var project = await this._documentStore.FindAsync<Project>(command.Id);

            var projectConfigs = project.Relevance(this._storage.AsQueryable<Config>()
                                                                .Where(s => command.ConfigIds.Contains(s.Id))
                                                                .ToArray());

            await this._documentStore.SaveAsync(projectConfigs);
        }

        public Task Handle(BatchRemoveCommand<ProjectConfig> command)
        {
            return this._documentStore.RemoveAsync<ProjectConfig>(command.Ids);
        }

        public async Task<IPagedList<ProjectConfigDto>> Handle(ProjectConfigQueryCommand command)
        {
            var query = this._storage.AsQueryable<ProjectConfigDto>().Where(s => s.ProjectId == command.ProjectId);
            return PagedList.Build(query, command);
        }

        async Task<IPagedList<ConfigDto>> ICommandHandler<ProjectConfigQueryCommand, IPagedList<ConfigDto>>.Handle(ProjectConfigQueryCommand command)
        {
            var query = this._storage.AsQueryable<ConfigDto>().Where(s => s.SolutionId == command.SolutionId);
            var configIds = this._storage.AsQueryable<ProjectConfig>()
                               .Where(s => s.ProjectId == command.ProjectId)
                               .Select(s => s.ConfigId)
                               .ToArray();
            query = query.Where(s => !configIds.Contains(s.Id));
            return PagedList.Build(query,command);
        }
    }
}
