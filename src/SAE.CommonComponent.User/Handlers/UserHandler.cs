using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.User.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Domains;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.User.Handlers
{
    public class UserHandler : AbstractHandler<Domains.User>,
                              ICommandHandler<UserCommand.ChangePassword>,
                              ICommandHandler<UserCommand.ChangeStatus>,
                              ICommandHandler<UserCommand.GetByName, UserDto>,
                              ICommandHandler<UserCommand.Query, IPagedList<UserDto>>,
                              ICommandHandler<Command.Find<UserDto>, UserDto>,
                              ICommandHandler<UserCommand.Authentication, UserDto>,
                              ICommandHandler<UserCommand.Create, string>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;

        public UserHandler(IDocumentStore documentStore,
                          IStorage storage,
                          IMediator mediator,
                          SAE.CommonLibrary.ObjectMapper.IObjectMapper objectMapper) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
        }

        public async Task<string> HandleAsync(UserCommand.Create command)
        {
            var user = new Domains.User(command);
            await user.AccountExist(this.AccountExist);
            await this._documentStore.SaveAsync(user);
            return user.Id;
        }

        public async Task HandleAsync(UserCommand.ChangePassword command)
        {
            var user = await this.FindAsync(command.Id);
            user.ChangePassword(command);
            await this._documentStore.SaveAsync(user);
        }

        public Task HandleAsync(UserCommand.ChangeStatus command)
        {
            return this.UpdateAsync(command.Id, user =>
            {
                user.ChangeStatus(command);
            });
        }

        public Task<UserDto> HandleAsync(UserCommand.GetByName command)
        {
            var user = this._storage.AsQueryable<UserDto>()
                            .FirstOrDefault(s => s.Account.Equals(command.AccountName));
            return Task.FromResult(user);
        }

        public Task<IPagedList<UserDto>> HandleAsync(UserCommand.Query command)
        {
            var query = this._storage.AsQueryable<UserDto>();
            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task<UserDto> HandleAsync(Command.Find<UserDto> command)
        {
            return this._storage.AsQueryable<UserDto>()
                        .FirstOrDefault(s => s.Id == command.Id);

        }

        public async Task<UserDto> HandleAsync(UserCommand.Authentication command)
        {
            var dto = this._storage.AsQueryable<UserDto>()
                                   .FirstOrDefault(s => s.Account.Name == command.AccountName);

            if (dto != null)
            {
                var user = await this._documentStore.FindAsync<Domains.User>(dto.Id);
                if (user == null || !user.Authentication(command.Password))
                {
                    dto = null;
                }
            }

            return dto;
        }

        /// <summary>
        /// Whether the account exists 
        /// </summary>
        /// <param name="name">account name</param>
        /// <returns></returns>
        private async Task<bool> AccountExist(string name)
        {
            return (await this.HandleAsync(new UserCommand.GetByName
            {
                AccountName = name
            })) != null;
        }
    }
}
