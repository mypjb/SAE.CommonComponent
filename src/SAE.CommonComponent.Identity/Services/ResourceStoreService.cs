using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using SAE.CommonComponent.Identity.Commands;
using SAE.CommonComponent.Identity.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using System;

namespace SAE.CommonComponent.Identity.Services
{
    public class ResourceStoreService : IResourceStore
    {
        private readonly IMediator _mediator;
        private readonly IdentityOption _option;
        private List<IdentityResource> _identities;

        public ResourceStoreService(IMediator mediator, IdentityOption option)
        {
            this._mediator = mediator;
            this._option = option;
            this._identities = new List<IdentityResource>();
            this._identities.Add(new IdentityResources.Profile());
            this._identities.Add(new IdentityResources.OpenId());
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var scopes = await this._mediator.Send<IEnumerable<ScopeDto>>(new ScopeQueryALLCommand());
            return scopes.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = await this._mediator.Send<IEnumerable<ScopeDto>>(new ScopeQueryALLCommand());
            return scopes.Where(s => scopeNames.Any(p => s.Name.Equals(p, StringComparison.OrdinalIgnoreCase)))
                         .Select(s => (ApiResource)s);
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(this._identities.Where(s => scopeNames.Any(p => s.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase))));
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var scopes = await this._mediator.Send<IEnumerable<ScopeDto>>(new ScopeQueryALLCommand());
            return new Resources(this._identities, scopes.Select(s => (ApiResource)s).ToArray());
        }
    }
}