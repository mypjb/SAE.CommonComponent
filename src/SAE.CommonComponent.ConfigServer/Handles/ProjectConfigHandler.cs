using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
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
                                        ICommandHandler<ProjectConfigQueryCommand, IPagedList<ConfigDto>>,
                                        ICommandHandler<ProjectConfigChangeAliasCommand>
    {
        private readonly IMediator _mediator;
        private readonly IStorage _storage;
        public ProjectConfigHandler(IDocumentStore documentStore, IStorage storage, IMediator mediator) : base(documentStore)
        {
            this._mediator = mediator;
            this._storage = storage;
        }

        public async Task Handle(ProjectRelevanceConfigCommand command)
        {
            if (!command.ConfigIds?.Any() ?? false)
            {
                return;
            }

            var project = await this._documentStore.FindAsync<Project>(command.ProjectId);

            var projectConfigs = project.Relevance(this._storage.AsQueryable<Config>()
                                                                .Where(s => command.ConfigIds.Contains(s.Id))
                                                                .ToArray());

                                                                
            await this._documentStore.SaveAsync(projectConfigs);

            await this._mediator.Send(new ProjectVersionCumulationCommand
            {
                ProjectId = project.Id
            });
        }

        public async Task Handle(BatchRemoveCommand<ProjectConfig> command)
        {
            if (!command.Ids?.Any() ?? false)
            {
                return;
            }
            await this._documentStore.RemoveAsync<ProjectConfig>(command.Ids);
            var projectId = command.Ids.First().Split('_').First();
            await this._mediator.Send(new ProjectVersionCumulationCommand
            {
                ProjectId = projectId
            });
        }

        public async Task<IPagedList<ProjectConfigDto>> Handle(ProjectConfigQueryCommand command)
        {
            var query = this._storage.AsQueryable<ProjectConfigDto>().Where(s => s.ProjectId == command.ProjectId);
            return PagedList.Build(query, command);
        }

        public async Task Handle(ProjectConfigChangeAliasCommand command)
        {
            var projectConfig = await this._documentStore.FindAsync<ProjectConfig>(command.Id);

            projectConfig.Change(command);

            Assert.Build(this._storage.AsQueryable<ProjectConfigDto>()
                             .Where(s => s.ProjectId == command.Id && s.Alias == command.Name)
                             .Count()>0)
                  .True($"{command.Name} is exist");

            await this._mediator.Send(new ProjectVersionCumulationCommand
            {
                ProjectId = projectConfig.ProjectId
            });
        }

        async Task<IPagedList<ConfigDto>> ICommandHandler<ProjectConfigQueryCommand, IPagedList<ConfigDto>>.Handle(ProjectConfigQueryCommand command)
        {
            var query = this._storage.AsQueryable<ConfigDto>().Where(s => s.SolutionId == command.SolutionId);
            var configIds = this._storage.AsQueryable<ProjectConfig>()
                               .Where(s => s.ProjectId == command.ProjectId)
                               .Select(s => s.ConfigId)
                               .ToArray();
            query = query.Where(s => !configIds.Contains(s.Id));
            return PagedList.Build(query, command);
        }
    }
}
