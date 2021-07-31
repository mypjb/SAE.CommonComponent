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
using AccessCredentialsCommand = SAE.CommonComponent.Application.Commands.AccessCredentialsCommand;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{
    public class AccessCredentialsHandler : AbstractHandler,
                              ICommandHandler<AccessCredentialsCommand.Create, string>,
                              ICommandHandler<AccessCredentialsCommand.Change>,
                              ICommandHandler<Command.Delete<AccessCredentials>>,
                              ICommandHandler<AccessCredentialsCommand.ChangeStatus>,
                              ICommandHandler<AccessCredentialsCommand.RefreshSecret, string>,
                              ICommandHandler<AccessCredentialsCommand.Query, IPagedList<AccessCredentialsDto>>,
                              ICommandHandler<Command.Find<AccessCredentialsDto>, AccessCredentialsDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly ILogging<AccessCredentialsHandler> _logging;
        public AccessCredentialsHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IMediator mediator,
                          ILogging<AccessCredentialsHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
        }


        public Task HandleAsync(AccessCredentialsCommand.Change command)
        {
            return this.UpdateAsync<AccessCredentials>(command.Id, app => app.Change(command));
        }

        public async Task<string> HandleAsync(AccessCredentialsCommand.Create command)
        {
            var app = await this.AddAsync(new AccessCredentials(command));
            return app.Id;
        }

        public Task HandleAsync(AccessCredentialsCommand.ChangeStatus command)
        {
            return this.UpdateAsync<AccessCredentials>(command.Id, app => app.ChangeStatus(command));
        }

        public async Task<string> HandleAsync(AccessCredentialsCommand.RefreshSecret command)
        {
            var secret = string.Empty;
            await this.UpdateAsync<AccessCredentials>(command.Id, app =>
            {
                app.RefreshSecret();
                secret = app.Secret;
            });
            return secret;
        }

        public async Task<AccessCredentialsDto> HandleAsync(Command.Find<AccessCredentialsDto> command)
        {
            var dto = this._storage.AsQueryable<AccessCredentialsDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public Task<IPagedList<AccessCredentialsDto>> HandleAsync(AccessCredentialsCommand.Query command)
        {
            var query = this._storage.AsQueryable<AccessCredentialsDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task HandleAsync(Command.Delete<AccessCredentials> command)
        {
            await this.DeleteAsync<AccessCredentials>(command.Id);
        }
    }
}