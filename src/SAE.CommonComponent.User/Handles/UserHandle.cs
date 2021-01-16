using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.User.Abstract.Dtos;
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

namespace SAE.CommonComponent.User.Handles
{
    public class UserHandle : AbstractHandler<Domains.User>,
                              ICommandHandler<UserCommand.Register, string>,
                              ICommandHandler<UserCommand.ChangePassword>,
                              ICommandHandler<UserCommand.ChangeStatus>,
                              ICommandHandler<UserCommand.GetByName, UserDto>,
                              ICommandHandler<UserCommand.Query, IPagedList<UserDto>>,
                              ICommandHandler<Command.Find<UserDto>, UserDto>,
                              ICommandHandler<UserCommand.Authentication, UserDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;

        public UserHandle(IDocumentStore documentStore, 
                          IStorage storage,
                          IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
        }

        public async Task<string> Handle(UserCommand.Register command)
        {
                var user = new Domains.User(command);
                await user.AccountExist(this.AccountExist);
                await this._documentStore.SaveAsync(user);
                return user.Id;
        }

        public async Task Handle(UserCommand.ChangePassword command)
        {
            var user = await this.GetById(command.Id);
            user.ChangePassword(command);
            await this._documentStore.SaveAsync(user);
        }

        public Task Handle(UserCommand.ChangeStatus command)
        {
            return this.Update(command.Id, user =>
            {
                user.ChangeStatus(command);
            });
        }

        public Task<UserDto> Handle(UserCommand.GetByName command)
        {
            var user = this._storage.AsQueryable<UserDto>()
                           .FirstOrDefault(s => s.AccountName.Equals(command.AccountName));
            return Task.FromResult(user);
        }

        public Task<IPagedList<UserDto>> Handle(UserCommand.Query command)
        {
            var query = this._storage.AsQueryable<UserDto>();
            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task<UserDto> Handle(Command.Find<UserDto> command)
        {
            return this._storage.AsQueryable<UserDto>()
                                 .FirstOrDefault(s => s.Id == command.Id);

        }

        public async Task<UserDto> Handle(UserCommand.Authentication command)
        {
            var dto = this._storage.AsQueryable<UserDto>()
                                   .FirstOrDefault(s => s.AccountName == command.AccountName);

            if (dto != null)
            {
                var user = await this._documentStore.FindAsync<Domains.User>(dto.Id);
                if (user == null || !user.Authentication(command.Password))
                {
                    dto = null;
                }
                else
                {
                    var code = await this._mediator.Send<string>(new UserRoleCommand.QueryUserAuthorizeCode
                    {
                        UserId = dto.Id
                    });

                    dto.AuthorizeCode = code;
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
            return (await this.Handle(new UserCommand.GetByName
            {
                AccountName = name
            })) != null;
        }
    }
}
