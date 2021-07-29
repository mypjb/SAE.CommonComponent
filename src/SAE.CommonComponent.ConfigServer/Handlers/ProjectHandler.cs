using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Extension;
using System.Collections.Generic;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary.Abstract.Builder;

namespace SAE.CommonComponent.ConfigServer.Handlers
{
    public class ProjectHandler : AbstractHandler<Project>,
                                  ICommandHandler<ProjectCommand.Create, string>,
                                  ICommandHandler<ProjectCommand.Change>,
                                  ICommandHandler<Command.Delete<Project>>,
                                  ICommandHandler<Command.Find<ProjectDto>, ProjectDto>,
                                  ICommandHandler<Command.Find<ProjectDetailDto>, ProjectDetailDto>,
                                  ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDto>>,
                                  ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDetailDto>>,
                                  ICommandHandler<ProjectCommand.Publish>,
                                  ICommandHandler<ProjectCommand.Preview, ProjectPreviewDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;

        public ProjectHandler(IDocumentStore documentStore,
                              IStorage storage,
                              IMediator mediator,
                              IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
        }

        public async Task<string> HandleAsync(ProjectCommand.Create command)
        {
            var project = await this.AddAsync(new Project(command));
            return project.Id;
        }

        public Task HandleAsync(ProjectCommand.Change command)
        {
            return this.UpdateAsync(command.Id, s => s.Change(command));
        }

        public Task HandleAsync(Command.Delete<Project> command)
        {
            return this.DeleteAsync(command.Id);
        }

        public Task<ProjectDto> HandleAsync(Command.Find<ProjectDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<ProjectDto>()
                       .FirstOrDefault(s => s.Id == command.Id));
        }

        public async Task<ProjectDetailDto> HandleAsync(Command.Find<ProjectDetailDto> command)
        {
            var projectDto = await this._mediator.SendAsync<ProjectDto>(command.To<Command.Find<ProjectDto>>());

            var projectDetailDto =projectDto.To<ProjectDetailDto>();

            Assert.Build(projectDetailDto)
                  .NotNull();

            await this._director.Build<IEnumerable<ProjectDetailDto>>(new[] { projectDetailDto });

            return projectDetailDto;
        }

        public async Task<IPagedList<ProjectDto>> HandleAsync(ProjectCommand.Query command)
        {

            var query = this._storage.AsQueryable<ProjectDto>();
            if (!command.SolutionId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.SolutionId == command.SolutionId);
            }
            if (command.IgnoreIds?.Any() ?? false)
            {
                query = query.Where(s => !command.IgnoreIds.Contains(s.Id));
            }
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }

        async Task<IPagedList<ProjectDetailDto>> ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDetailDto>>.HandleAsync(ProjectCommand.Query command)
        {
            var paging = await this._mediator.SendAsync<IPagedList<ProjectDto>>(command);
            var projectDetails = paging.Select(s => s.To<ProjectDetailDto>());
            await this._director.Build(projectDetails);
            return PagedList.Build(projectDetails, paging);
        }

        public async Task HandleAsync(ProjectCommand.Publish command)
        {
            var tuple = await this.CombinationConfigAsync(command);

            var projectData = this._storage.AsQueryable<ProjectData>()
                                           .FirstOrDefault(s => s.ProjectId == command.Id &&
                                                           s.EnvironmentId == command.EnvironmentId);

            if (projectData == null)
            {
                projectData = new ProjectData(new ProjectDataEvent.Create
                {
                    ProjectId = command.Id,
                    EnvironmentId = command.EnvironmentId,
                    Data = tuple.Item2.ToJsonString(),
                    PublicData = tuple.Item1.ToJsonString(),
                });
            }
            else
            {
                projectData = await this._documentStore.FindAsync<ProjectData>(projectData.Id);
                projectData.Change(new ProjectDataEvent.Publish
                {
                    Data = tuple.Item2.ToJsonString(),
                    PublicData = tuple.Item1.ToJsonString()
                });
            }

            await this._documentStore.SaveAsync(projectData);
        }

        public async Task<ProjectPreviewDto> HandleAsync(ProjectCommand.Preview command)
        {
            var tuple = await this.CombinationConfigAsync(command);
            return new ProjectPreviewDto
            {
                Public = tuple.Item1,
                Private = tuple.Item2
            };
        }



        private async Task<Tuple<IDictionary<string, object>, IDictionary<string, object>>> CombinationConfigAsync(ProjectCommand.Publish command)
        {
            var project = await this.FindAsync(command.Id);

            var projectConfigs = this._storage.AsQueryable<ProjectConfigDto>()
                                          .Where(s => s.ProjectId == command.Id &&
                                                 s.EnvironmentId == command.EnvironmentId)
                                          .ToArray();

            var configIds = projectConfigs.Select(p => p.ConfigId).ToArray();

            var configs = this._storage.AsQueryable<ConfigDto>()
                                       .Where(s => configIds.Contains(s.Id))
                                       .ToArray();

            var privateData = new Dictionary<string, object>();

            var publicData = new Dictionary<string, object>();

            foreach (var projectConfig in projectConfigs.Where(s => configs.Any(c => c.Id == s.ConfigId))
                                                                .ToArray())
            {
                var config = configs.FirstOrDefault(s => s.Id == projectConfig.ConfigId);

                var key = projectConfig.Alias;

                if (privateData.ContainsKey(key))
                {
                    key += "_";
                    privateData[key] = config?.Content.ToObject<object>();
                }
                else
                {
                    privateData[key] = config?.Content.ToObject<object>();
                }

                if (!projectConfig.Private)
                {
                    publicData[key] = privateData[key];
                }

            }

            return new Tuple<IDictionary<string, object>, IDictionary<string, object>>(publicData, privateData);
        }

        
    }

}
