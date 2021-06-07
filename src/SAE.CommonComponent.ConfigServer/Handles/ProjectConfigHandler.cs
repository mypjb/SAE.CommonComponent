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
                                        ICommandHandler<ProjectCommand.RelevanceConfig>,
                                        ICommandHandler<Command.BatchDelete<ProjectConfig>>,
                                        ICommandHandler<ProjectCommand.ConfigQuery, IPagedList<ProjectConfigDto>>,
                                        ICommandHandler<ProjectCommand.ConfigQuery, IPagedList<ConfigDto>>,
                                        ICommandHandler<ProjectCommand.ConfigChangeAlias>,
                                        ICommandHandler<Command.Find<ProjectConfigDto>, ProjectConfigDto>
    {
        private readonly IMediator _mediator;
        private readonly IStorage _storage;
        public ProjectConfigHandler(IDocumentStore documentStore, IStorage storage, IMediator mediator) : base(documentStore)
        {
            this._mediator = mediator;
            this._storage = storage;
        }

        public async Task HandleAsync(ProjectCommand.RelevanceConfig command)
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
        }

        public async Task HandleAsync(Command.BatchDelete<ProjectConfig> command)
        {
            if (!command.Ids?.Any() ?? false)
            {
                return;
            }
            await this._documentStore.DeleteAsync<ProjectConfig>(command.Ids);
        }

        public async Task<IPagedList<ProjectConfigDto>> HandleAsync(ProjectCommand.ConfigQuery command)
        {
            var query = this._storage.AsQueryable<ProjectConfigDto>()
                                     .Where(s => s.ProjectId == command.ProjectId &&
                                            s.EnvironmentId == command.EnvironmentId);
            return PagedList.Build(query, command);
        }

        public async Task HandleAsync(ProjectCommand.ConfigChangeAlias command)
        {
            var projectConfig = await this._documentStore.FindAsync<ProjectConfig>(command.Id);

            projectConfig.Change(command);

            Assert.Build(this._storage.AsQueryable<ProjectConfigDto>()
                             .Where(s => s.ProjectId == projectConfig.ProjectId &&
                                    s.EnvironmentId == projectConfig.EnvironmentId &&
                                    s.Id != command.Id &&
                                    s.Alias == command.Alias)
                             .Count() == 0)
                  .True($"{command.Alias} is exist");

            await this._documentStore.SaveAsync(projectConfig);

            //await this._mediator.SendAsync(new ProjectCommand.VersionCumulation
            //{
            //    ProjectId = projectConfig.ProjectId
            //});
        }

        public async Task<ProjectConfigDto> HandleAsync(Command.Find<ProjectConfigDto> command)
        {
            return this._storage.AsQueryable<ProjectConfigDto>().First(s => s.Id == command.Id);
        }

        async Task<IPagedList<ConfigDto>> ICommandHandler<ProjectCommand.ConfigQuery, IPagedList<ConfigDto>>.HandleAsync(ProjectCommand.ConfigQuery command)
        {
            var project = await this._documentStore.FindAsync<Project>(command.ProjectId);

            Assert.Build(project).IsNotNull();

            var query = this._storage.AsQueryable<ConfigDto>().Where(s => s.SolutionId == project.SolutionId);

            var configIds = this._storage.AsQueryable<ProjectConfig>()
                               .Where(s => s.ProjectId == command.ProjectId &&
                                      s.EnvironmentId == command.EnvironmentId)
                               .Select(s => s.ConfigId)
                               .ToArray();

            query = query.Where(s => !configIds.Contains(s.Id));

            return PagedList.Build(query, command);
        }
    }
}
