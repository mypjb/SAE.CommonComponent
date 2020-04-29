using SAE.CommonComponent.Identity.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Handlers
{
    public class AccountHandle : ICommandHandler<AccountLoginCommand, IPrincipal>
    {
        public Task<IPrincipal> Handle(AccountLoginCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}
