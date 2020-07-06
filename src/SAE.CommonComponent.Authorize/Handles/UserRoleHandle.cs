using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SAE.CommonComponent.Authorize.Handles
{
    public class UserRoleHandle : AbstractHandler<UserRole>,
                                  ICommandHandler<UserRoleCommand.Reference>,
                                  ICommandHandler<UserRoleCommand.DeleteReference>,
                                  ICommandHandler<Command.List<UserRole>,IEnumerable<Permission>>,
                                  ICommandHandler<Command.List<UserRole>, IEnumerable<Role>>,
                                  ICommandHandler<Command.List<UserRole>, IEnumerable<BitmapEndpoint>>
    {
        public UserRoleHandle(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public Task Handle(UserRoleCommand.Reference command)
        {
            throw new NotImplementedException();
        }

        public Task Handle(UserRoleCommand.DeleteReference command)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role>> Handle(Command.List<UserRole> command)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<BitmapEndpoint>> ICommandHandler<Command.List<UserRole>, IEnumerable<BitmapEndpoint>>.Handle(Command.List<UserRole> command)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Permission>> ICommandHandler<Command.List<UserRole>, IEnumerable<Permission>>.Handle(Command.List<UserRole> command)
        {
            throw new NotImplementedException();
        }
    }
}
