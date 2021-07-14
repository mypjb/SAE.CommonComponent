using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Services
{
    public class ClaimsService : DefaultClaimsService, IClaimsService
    {
        public ClaimsService(IProfileService profile, ILogger<DefaultClaimsService> logger) : base(profile, logger)
        {

        }

        public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject,
            ResourceValidationResult resourceResult,
            ValidatedRequest request)
        {
            var claims=(await base.GetAccessTokenClaimsAsync(subject, resourceResult, request)).ToList();
            foreach (var claim in this.GetCustomClaim(subject))
            {
                if (!claims.Any(s => s.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase)))
                {
                    claims.Add(claim);
                }
            }

            claims.Add(new Claim(CommonLibrary.AspNetCore.Constants.Administrator, "1", Constants.Claim.CustomType));

            return claims;
        }

        public override async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject,
            ResourceValidationResult resources, 
            bool includeAllIdentityClaims, 
            ValidatedRequest request)
        {
            var claims= await base.GetIdentityTokenClaimsAsync(subject, resources, includeAllIdentityClaims, request);
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
