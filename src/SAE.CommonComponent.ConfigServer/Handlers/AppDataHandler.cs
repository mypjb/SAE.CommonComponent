using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class AppDataHandler : ICommandHandler<AppDataCommand.Find, AppDataDto>
    {
        private readonly IStorage _storage;
        private readonly IDocumentStore _documentStore;
        private readonly IMediator _mediator;

        public AppDataHandler(IStorage storage,
                                IDocumentStore documentStore,
                                IMediator mediator)
        {
            this._storage = storage;
            this._documentStore = documentStore;
            this._mediator = mediator;
        }
        public async Task<AppDataDto> HandleAsync(AppDataCommand.Find command)
        {
            var app = new AppDataDto();

            var envs = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Environment
            });

            var env = envs.FirstOrDefault(s => s.Name == command.Env);

            Assert.Build(env)
                  .NotNull($"env '{command.Env}' not exist!");

            //var dto = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto> { Id = command.AppId });

            var appData = this._storage.AsQueryable<AppConfigData>()
                                           .FirstOrDefault(s => s.AppId == command.AppId &&
                                                                s.EnvironmentId == env.Id);

            Assert.Build(appData)
                  .NotNull($"app '{command.AppId}' not exist!");

            app.Version = appData.Version;
            if (appData != null && appData.Version != command.Version)
            {
                app.Data = (command.Private ? appData.Data :
                                              appData.PublicData)
                                                         .ToObject<Dictionary<string, object>>();
            }

            return app;
        }
    }
}
