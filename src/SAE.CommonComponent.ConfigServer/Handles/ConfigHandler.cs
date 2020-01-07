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
    public class ConfigHandler : AbstractHandler<Config>,
                                 IRequestHandler<ConfigCreateCommand,string>,
                                 ICommandHandler<ConfigChangeCommand>,
                                 ICommandHandler<RemoveCommand<Config>>,
                                 IRequestHandler<GetByIdCommand<Config>, ConfigDto>
    {
        public ConfigHandler(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public Task<ConfigDto> Handle(GetByIdCommand<Config> command)
        {
            throw new NotImplementedException();
        }

        public Task Handle(RemoveCommand<Config> command)
        {
            return this.Remove(command.Id);
        }

        public async Task<string> Handle(ConfigCreateCommand command)
        {
            var config = await this.Add(new Config(command));
            return config.Id;
        }

        public Task Handle(ConfigChangeCommand command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }
    }
}
