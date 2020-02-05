using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class SolutionHandler : AbstractHandler<Solution>,
                                  ICommandHandler<SolutionCreateCommand, string>,
                                  ICommandHandler<SolutionChangeCommand>,
                                  ICommandHandler<RemoveCommand<Solution>>,
                                  ICommandHandler<string, SolutionDto>,
                                  ICommandHandler<ConfigQueryCommand, IPagedList<SolutionDto>>
    {
        public SolutionHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore, storage)
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

        public Task<SolutionDto> Handle(string command)
        {
            return Task.FromResult(this._storage.AsQueryable<SolutionDto>()
                           .FirstOrDefault(s => s.Id == command));
        }

        public async Task<IPagedList<SolutionDto>> Handle(ConfigQueryCommand command)
        {
            return PagedList.Build(this._storage.AsQueryable<SolutionDto>(), command);
        }
    }
}
