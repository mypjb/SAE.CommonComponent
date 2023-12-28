using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Services
{
    public class ClaimsService : DefaultClaimsService, IClaimsService
    {
        private readonly IMediator _mediator;

        public ClaimsService(IProfileService profile,
                             ILogger<DefaultClaimsService> logger,
                             IMediator mediator) : base(profile, logger)
        {
            this._mediator = mediator;
        }

        public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject,
            ResourceValidationResult resourceResult,
            ValidatedRequest request)
        {
            var claims = (await base.GetAccessTokenClaimsAsync(subject, resourceResult, request)).ToList();
            
#warning 专用于测试
            //claims.Add(new Claim(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Administrator, "1", Constants.Claim.CustomType));

            var client = await this._mediator.SendAsync<ClientDto>(new Command.Find<ClientDto> { Id = request.ClientId });

            claims.Add(new Claim(Constants.Claim.AppId, client.AppId, Constants.Claim.CustomType));

            foreach (var claim in this.GetCustomClaim(subject))
            {
                if (!claims.Any(s => s.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase)))
                {
                    claims.Add(claim);
                }
            }

            // if (!claims.Any(s => s.Type == CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim))
            // {
            //     var clientCodes = await this._mediator.SendAsync<Dictionary<string, string>>(new ClientRoleCommand.QueryClientAuthorizeCode
            //     {
            //         ClientId = client.Id
            //     });

            //     foreach (var kv in clientCodes)
            //     {
            //         claims.Add(new Claim(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.Claim,
            //                              string.Format(CommonLibrary.AspNetCore.Constants.BitmapAuthorize.GroupFormat,
            //                                            kv.Key,
            //                                            kv.Value)));
            //     }
            // }
            return claims;
        }

        public override async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject,
            ResourceValidationResult resources,
            bool includeAllIdentityClaims,
            ValidatedRequest request)
        {
            var claims = await base.GetIdentityTokenClaimsAsync(subject, resources, includeAllIdentityClaims, request);
            return claims;
        }

        /// <summary>
        /// 获得自定义Claim
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Claim> GetCustomClaim(ClaimsPrincipal claimsPrincipal)
        {

            var claims = claimsPrincipal == null ? new List<Claim>() :
                         claimsPrincipal.FindAll(c => c.ValueType.Equals(Constants.Claim.CustomType))
                                        .ToList();
            return claims;
        }

    }
}
