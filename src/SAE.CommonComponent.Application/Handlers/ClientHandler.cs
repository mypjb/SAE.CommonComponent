using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClientCommand = SAE.CommonComponent.Application.Commands.ClientCommand;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class ClientHandler : AbstractHandler,
                              ICommandHandler<ClientCommand.Create, string>,
                              ICommandHandler<ClientCommand.Change>,
                              ICommandHandler<Command.Delete<Client>>,
                              ICommandHandler<ClientCommand.ChangeStatus>,
                              ICommandHandler<ClientCommand.RefreshSecret, string>,
                              ICommandHandler<ClientCommand.Query, IPagedList<ClientDto>>,
                              ICommandHandler<Command.Find<ClientDto>, ClientDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly ILogging<ClientHandler> _logging;
        public ClientHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IMediator mediator,
                          ILogging<ClientHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
        }


        public Task HandleAsync(ClientCommand.Change command)
        {
            return this.UpdateAsync<Client>(command.Id, app => app.Change(command));
        }

        public async Task<string> HandleAsync(ClientCommand.Create command)
        {
            var app = await this.AddAsync(new Client(command));
            return app.Id;
        }

        public Task HandleAsync(ClientCommand.ChangeStatus command)
        {
            return this.UpdateAsync<Client>(command.Id, app => app.ChangeStatus(command));
        }

        public async Task<string> HandleAsync(ClientCommand.RefreshSecret command)
        {
            var secret = string.Empty;
            await this.UpdateAsync<Client>(command.Id, app =>
            {
                app.RefreshSecret();
                secret = app.Secret;
            });
            return secret;
        }

        public async Task<ClientDto> HandleAsync(Command.Find<ClientDto> command)
        {
            var dto = this._storage.AsQueryable<ClientDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public Task<IPagedList<ClientDto>> HandleAsync(ClientCommand.Query command)
        {
            var query = this._storage.AsQueryable<ClientDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task HandleAsync(Command.Delete<Client> command)
        {
            await this.DeleteAsync<Client>(command.Id);
        }
    }
}