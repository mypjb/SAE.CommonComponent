using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class EnvironmentVariableHandler : AbstractHandler<EnvironmentVariable>,
                                              ICommandHandler<EnvironmentVariableCommand.Create, string>,
                                              ICommandHandler<EnvironmentVariableCommand.Change>,
                                              ICommandHandler<EnvironmentVariableCommand.Query, IPagedList<EnvironmentVariableDto>>,
                                              ICommandHandler<Command.Delete<EnvironmentVariable>>,
                                              ICommandHandler<Command.Find<EnvironmentVariableDto>, EnvironmentVariableDto>,
                                              ICommandHandler<Command.List<EnvironmentVariableDto>, IEnumerable<EnvironmentVariableDto>>
                                              
    {
        private readonly IStorage _storage;

        public EnvironmentVariableHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;

        }

        public Task<EnvironmentVariableDto> Handle(Command.Find<EnvironmentVariableDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<EnvironmentVariableDto>()
                     .FirstOrDefault(s => s.Id == command.Id));
        }

        public Task Handle(Command.Delete<EnvironmentVariable> command)
        {
            return this.Remove(command.Id);
        }

        public async Task<string> Handle(EnvironmentVariableCommand.Create command)
        {
            var config = await this.Add(new EnvironmentVariable(command));
            return config.Id;
        }

        public Task Handle(EnvironmentVariableCommand.Change command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }


        public  Task<IEnumerable<EnvironmentVariableDto>> Handle(Command.List<EnvironmentVariableDto> command)
        {
            var result = this._storage.AsQueryable<EnvironmentVariableDto>()
                                       .ToList()
                                       .AsEnumerable();

            return Task.FromResult(result);
        }

        public async Task<IPagedList<EnvironmentVariableDto>> Handle(EnvironmentVariableCommand.Query command)
        {
            var query = this._storage.AsQueryable<EnvironmentVariableDto>();
            
            return PagedList.Build(query, command);
        }
    }
}
