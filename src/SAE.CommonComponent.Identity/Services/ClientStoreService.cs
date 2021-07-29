using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
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
            var app = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto> { Id = clientId });

            var dicts = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Scope
            });

            var scopes = dicts.Where(d => app.Scopes.Contains(d.Id))
                              .Select(d => d.Name)
                              .ToList();
            scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            scopes.Add(IdentityServerConstants.StandardScopes.Profile);
            
            return new Client
            {
                ClientId = app.Id,
                ClientName = app.Name,
                ClientSecrets = new[]
                {
                    new Secret(app.Secret.Sha256())
                },
                AllowedScopes = scopes.Distinct().ToArray(),
                Enabled = app.Status == Status.Enable,
                AccessTokenType = AccessTokenType.Jwt,
                AuthorizationCodeLifetime = this._option.AuthorizationCodeLifetime,
                AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                AllowRememberConsent = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = app.Endpoint.RedirectUris.ToArray(),
                PostLogoutRedirectUris = app.Endpoint.PostLogoutRedirectUris.ToArray()
            };
        }
    }
}