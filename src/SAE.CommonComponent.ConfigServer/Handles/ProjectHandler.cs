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

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class ProjectHandler : AbstractHandler<Project>,
                                  ICommandHandler<ProjectCommand.Create, string>,
                                  ICommandHandler<ProjectCommand.Change>,
                                  ICommandHandler<Command.Delete<Project>>,
                                  ICommandHandler<Command.Find<ProjectDto>, ProjectDto>,
                                  ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDto>>,
                                  ICommandHandler<ProjectCommand.Publish>,
                                  ICommandHandler<ProjectCommand.Preview, object>
    {
        private readonly IStorage _storage;

        public ProjectHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
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

        public async Task<IPagedList<ProjectDto>> HandleAsync(ProjectCommand.Query command)
        {
            var query = this._storage.AsQueryable<ProjectDto>().Where(s => s.SolutionId == command.SolutionId);
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }

        public async Task HandleAsync(ProjectCommand.Publish command)
        {
            var data = await this.CombinationConfigAsync(command);

            var projectData = this._storage.AsQueryable<ProjectData>()
                                           .FirstOrDefault(s => s.ProjectId == command.Id &&
                                                           s.EnvironmentId == command.EnvironmentId);

            if (projectData == null)
            {
                projectData = new ProjectData(new ProjectDataEvent.Create
                {
                    ProjectId = command.Id,
                    EnvironmentId = command.EnvironmentId,
                    Data = data.ToJsonString()
                });
            }
            else
            {
                projectData = await this._documentStore.FindAsync<ProjectData>(projectData.Id);
                projectData.Change(data.ToJsonString());
            }

            await this._documentStore.SaveAsync(projectData);
        }

        public async Task<object> HandleAsync(ProjectCommand.Preview command)
        {
            var data = await this.CombinationConfigAsync(command);
            return data;
        }


        private async Task<IDictionary<string, object>> CombinationConfigAsync(ProjectCommand.Publish command)
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

            var data = new Dictionary<string, object>();

            foreach (var projectConfig in projectConfigs.Where(s => configs.Any(c => c.Id == s.ConfigId))
                                                                .ToArray())
            {
                var config = configs.FirstOrDefault(s => s.Id == projectConfig.ConfigId);

                var key = projectConfig.Alias;

                if (data.ContainsKey(key))
                {

                    key += "_";
                    data[key] = config?.Content.ToObject<object>();
                }
                else
                {
                    data[key] = config?.Content.ToObject<object>();
                }
            }
            return data;
        }
    }

}
