using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class ProjectHandler : AbstractHandler<Project>,
                                  IRequestHandler<ProjectCreateCommand, string>,
                                  ICommandHandler<ProjectChangeCommand>,
                                  ICommandHandler<RemoveCommand<Project>>,
                                  IRequestHandler<GetByIdCommand<Project>, ProjectDto>
    {
        public ProjectHandler(IDocumentStore documentStore) : base(documentStore)
        {
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

        public Task<ProjectDto> Handle(GetByIdCommand<Project> command)
        {
            throw new NotImplementedException();
        }

       
    }
}
