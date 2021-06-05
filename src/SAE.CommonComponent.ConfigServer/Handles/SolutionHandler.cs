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
        ICommandHandler<SolutionCommand.Create, string>,
        ICommandHandler<SolutionCommand.Change>,
        ICommandHandler<Command.Delete<Solution>>,
        ICommandHandler<Command.Find<SolutionDto>, SolutionDto>,
        ICommandHandler<SolutionCommand.Query, IPagedList<SolutionDto>>
    {
        private readonly IStorage _storage;
        public SolutionHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task<string> HandleAsync(SolutionCommand.Create command)
        {
            var solution = await this.AddAsync(new Solution(command));
            return solution.Id;
        }

        public Task HandleAsync(SolutionCommand.Change command)
        {
            return this.UpdateAsync(command.Id, s => s.Change(command));
        }

        public Task HandleAsync(Command.Delete<Solution> command)
        {
            return this.DeleteAsync(command.Id);
        }

        public Task<SolutionDto> HandleAsync(Command.Find<SolutionDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<SolutionDto>()
                .FirstOrDefault(s => s.Id == command.Id));
        }

        public async Task<IPagedList<SolutionDto>> HandleAsync(SolutionCommand.Query command)
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