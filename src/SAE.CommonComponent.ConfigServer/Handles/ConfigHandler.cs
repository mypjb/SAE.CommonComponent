﻿using SAE.CommonComponent.ConfigServer.Commands;
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
    public class ConfigHandler : AbstractHandler<Config>,
                                 ICommandHandler<ConfigCreateCommand, string>,
                                 ICommandHandler<ConfigChangeCommand>,
                                 ICommandHandler<RemoveCommand<Config>>,
                                 ICommandHandler<string, ConfigDto>,
                                 ICommandHandler<ConfigQueryCommand, IPagedList<ConfigDto>>
    {
        public ConfigHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore, storage)
        {

        }

        public Task<ConfigDto> Handle(string command)
        {
            return Task.FromResult(this._storage.AsQueryable<ConfigDto>()
                     .FirstOrDefault(s => s.Id == command));
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

        public async Task<IPagedList<ConfigDto>> Handle(ConfigQueryCommand command)
        {
            return PagedList.Build(this._storage.AsQueryable<ConfigDto>()
                                                .Where(s => s.SolutionId == command.SolutionId), command);
        }
    }
}
