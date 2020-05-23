using SAE.CommonComponent.User.Abstract.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
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
                              ICommandHandler<UserCommand.Find,UserDto>,
                              ICommandHandler<UserCommand.Query,IPagedList<UserDto>>
    {
        public UserHandle(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<string> Handle(UserCommand.Register command)
        {
            var user = new Domains.User(command);
            await user.AccountExist(this.AccountExist);
            await this._documentStore.SaveAsync(user);

            return user.Account.Name;
        }

        public async Task Handle(UserCommand.ChangePassword command)
        {
            var user = await this.GetById(command.Id);
            
        }

        public Task Handle(UserCommand.ChangeStatus command)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> Handle(UserCommand.Find command)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<UserDto>> Handle(UserCommand.Query command)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Whether the account exists 
        /// </summary>
        /// <param name="name">account name</param>
        /// <returns></returns>
        private async Task<bool> AccountExist(string name)
        {
            return (await this.Handle(new UserCommand.Find
            {
                AccountName = name
            })) != null;
        }
    }
}
