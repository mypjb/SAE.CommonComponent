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
            var claims=await base.GetAccessTokenClaimsAsync(subject, resourceResult, request);
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
    }
}
