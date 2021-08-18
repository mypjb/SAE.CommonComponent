using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var client = await this._mediator.SendAsync<ClientDto>(new Command.Find<ClientDto> { Id = clientId });

            Assert.Build(client)
                  .NotNull();

            var dicts = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Scope
            });

            var scopes = dicts.Where(d => client.Scopes.Contains(d.Id))
                              .Select(d => d.Name)
                              .ToList();
            scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            scopes.Add(IdentityServerConstants.StandardScopes.Profile);
            
            return new Client
            {
                ClientId = client.Id,
                ClientName = client.Name,
                ClientSecrets = new[]
                {
                    new Secret(client.Secret.Sha256())
                },
                AllowedScopes = scopes.Distinct().ToArray(),
                Enabled = client.Status == Status.Enable,
                AccessTokenType = AccessTokenType.Jwt,
                AuthorizationCodeLifetime = this._option.AuthorizationCodeLifetime,
                AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                AllowRememberConsent = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = client.Endpoint.RedirectUris.ToArray(),
                PostLogoutRedirectUris = client.Endpoint.PostLogoutRedirectUris.ToArray()
            };
        }
    }
}