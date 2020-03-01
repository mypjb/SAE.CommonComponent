using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class SolutionHandler : AbstractHandler<Solution>,
        ICommandHandler<SolutionCreateCommand, string>,
        ICommandHandler<SolutionChangeCommand>,
        ICommandHandler<RemoveCommand<Solution>>,
        ICommandHandler<string, SolutionDto>,
        ICommandHandler<SolutionQueryCommand, IPagedList<SolutionDto>>
    {
        private readonly IStorage _storage;
        public SolutionHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
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

        public async Task<IPagedList<SolutionDto>> Handle(SolutionQueryCommand command)
        {
            var query = this._storage.AsQueryable<SolutionDto>();
            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return PagedList.Build(query, command);
        }
    }
}