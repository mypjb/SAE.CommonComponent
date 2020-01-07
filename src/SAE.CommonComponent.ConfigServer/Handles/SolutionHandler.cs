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
    public class SolutionHandler : AbstractHandler<Solution>,
                                  IRequestHandler<SolutionCreateCommand, string>,
                                  ICommandHandler<SolutionChangeCommand>,
                                  ICommandHandler<RemoveCommand<Solution>>,
                                  IRequestHandler<GetByIdCommand<Solution>, SolutionDto>
    {
        public SolutionHandler(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<string> Handle(SolutionCreateCommand command)
        {
            var solution = await this.Add(new Solution(command));
            return solution.Id;
        }

        public Task Handle(SolutionChangeCommand command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }

        public Task Handle(RemoveCommand<Solution> command)
        {
            return this.Remove(command.Id);
        }

        public Task<SolutionDto> Handle(GetByIdCommand<Solution> command)
        {
            throw new NotImplementedException();
        }
    }
}
