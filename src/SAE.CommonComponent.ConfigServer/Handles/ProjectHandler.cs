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
                                  ICommandHandler<ProjectCreateCommand, string>,
                                  ICommandHandler<ProjectChangeCommand>,
                                  ICommandHandler<RemoveCommand<Project>>,
                                  ICommandHandler<string, ProjectDto>,
                                  ICommandHandler<ProjectQueryCommand, IPagedList<ProjectDto>>
    {
        private readonly IStorage _storage;

        public ProjectHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task<string> Handle(ProjectCreateCommand command)
        {
            var project = await this.Add(new Project(command));
            return project.Id;
        }

        public Task Handle(ProjectChangeCommand command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }

        public Task Handle(RemoveCommand<Project> command)
        {
            return this.Remove(command.Id);
        }

        public Task<ProjectDto> Handle(string command)
        {
            return Task.FromResult(this._storage.AsQueryable<ProjectDto>()
                       .FirstOrDefault(s => s.Id == command));
        }

        public async Task<IPagedList<ProjectDto>> Handle(ProjectQueryCommand command)
        {
            var query = this._storage.AsQueryable<ProjectDto>().Where(s => s.SolutionId == command.SolutionId);
            if (command.Name.IsNotNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }
            return PagedList.Build(query, command);
        }
    }
}
