using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class ProjectHandler : AbstractHandler<Project>,
                                  IRequestHandler<ProjectCreateCommand,string>,
                                  ICommandHandler<ProjectChangeCommand>,
                                  ICommandHandler<RemoveCommand<Project>>,
                                  ICommandHandler<ProjectRelevanceConfigCommand>,
                                  IRequestHandler<GetByIdCommand<Project>, ProjectDto>
    {
        public ProjectHandler(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<string> Handle(ProjectCreateCommand command)
        {
            var project =await this.Add(new Project(command));
            return project.Id;
        }

        public Task Handle(ProjectChangeCommand command)
        {
            throw new NotImplementedException();
        }

        public Task Handle(RemoveCommand<Project> command)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDto> Handle(GetByIdCommand<Project> command)
        {
            throw new NotImplementedException();
        }

        public Task Handle(ProjectRelevanceConfigCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
