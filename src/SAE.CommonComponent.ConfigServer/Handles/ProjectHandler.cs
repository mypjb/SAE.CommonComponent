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

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class ProjectHandler : AbstractHandler<Project>,
                                  ICommandHandler<ProjectCommand.Create, string>,
                                  ICommandHandler<ProjectCommand.Change>,
                                  ICommandHandler<Command.Delete<Project>>,
                                  ICommandHandler<Command.Find<ProjectDto>, ProjectDto>,
                                  ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDto>>,
                                  ICommandHandler<ProjectCommand.VersionCumulation>
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

        public async Task HandleAsync(ProjectCommand.VersionCumulation command)
        {
           var project= await this._documentStore.FindAsync<Project>(command.ProjectId);
           project.Cumulation();
           await this._documentStore.SaveAsync(project);
        }
    }
}
