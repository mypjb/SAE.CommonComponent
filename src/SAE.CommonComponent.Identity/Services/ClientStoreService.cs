using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using SAE.CommonComponent.Identity.Domains;
using SAE.CommonComponent.Identity.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;

namespace SAE.CommonComponent.Identity.Services
{
    public class ClientStoreService : IClientStore
    {
        private readonly IMediator _mediator;
        private readonly IdentityOption _option;

        public ClientStoreService(IMediator mediator, IdentityOption option)
        {
            this._mediator = mediator;
            this._option = option;
        }
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var app = await this._mediator.Send<AppDto>(clientId);

            return new Client
            {
                ClientId = app.Id,
                ClientName = app.Name,
                ClientSecrets = new[]
                {
                    new Secret(app.Secret.Sha256())
                },
                AllowedScopes = app.Scopes.ToArray(),
                Enabled = app.Status == Status.Enable,
                AccessTokenType = AccessTokenType.Jwt,
                AuthorizationCodeLifetime = this._option.AuthorizationCodeLifetime,
                AllowedGrantTypes = GrantTypes.Hybrid,
                
            };
        }
    }
}