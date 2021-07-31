using IdentityServer4.Models;
using IdentityServer4.Stores;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var openId = new IdentityResources.OpenId();
            openId.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator);
            this._identities.Add(openId);
        }

        private async Task<IEnumerable<DictDto>> GetScopes()
        {
            var scopes = await this._mediator.SendAsync<IEnumerable<DictDto>>(new DictCommand.List
            {
                Type = (int)DictType.Scope
            });

            return scopes;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var scopes = await this.GetScopes();
            var scope = scopes.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (scope == null)
            {
                return null;
            }
            var apiResource = new ApiResource(scope.Name, scope.Name);
# warning 该处是否可以统一设置？不同的应用Claim可能不同
            apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator);
            apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim);
            return apiResource;
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            return this.FindApiResourcesByScopeNameAsync(apiResourceNames);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = await this.GetScopes();
            return scopes.Where(s => scopeNames.Any(p => s.Name.Equals(p, StringComparison.OrdinalIgnoreCase)))
                         .Select(s =>
                         {
                             var apiResource = new ApiResource(s.Name, s.Name);
#warning 该处是否可以统一设置？不同的应用Claim可能不同
                             apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator);
                             apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim);
                             return apiResource;
                         });
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = await this.GetScopes();
            return scopes.Select(s =>
            {
                var apiResource = new ApiScope(s.Name, s.Name);
#warning 该处是否可以统一设置？不同的应用Claim可能不同
                apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator);
                apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim);
                return apiResource;
            }).ToArray();
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(this._identities.Where(s => scopeNames.Any(p => s.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase))));
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var scopes = await this.GetScopes();
            return new Resources(this._identities,
                                 scopes.Select(s =>
                                 {
                                     var apiResource = new ApiResource(s.Name, s.Name);
#warning 该处是否可以统一设置？不同的应用Claim可能不同
                                     apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator);
                                     apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim);
                                     return apiResource;
                                 }).ToArray(),
                                 scopes.Select(s =>
                                 {
                                     var apiResource = new ApiScope(s.Name, s.Name);
#warning 该处是否可以统一设置？不同的应用Claim可能不同
                                     apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator);
                                     apiResource.UserClaims.Add(SAE.CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim);
                                     return apiResource;
                                 }).ToArray());
        }

    }
}