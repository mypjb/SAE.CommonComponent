using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
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

            var rootDict = await this._mediator.SendAsync<DictDto>(new DictCommand.Find
            {
                Names = Constants.Dict.LabelClient
            });

            if (rootDict != null)
            {
                var labels = await this._mediator.SendAsync<IEnumerable<LabelDto>>(new LabelResourceCommand.List
                {
                    ResourceId = request.ClientId,
                    ResourceType = rootDict.Id
                });

                foreach (var label in labels)
                {
                    if (!claims.Any(s => s.Type.Equals(label.Name) &&
                                            s.Value.Equals(label.Value)))
                    {
                        claims.Add(new Claim($"{Constants.Authorize.CustomPrefix}{label.Name}", label.Value, Constants.Claim.CustomType));
                    }
                }
            }

            await this.AppendClaimsAsync(subject, request, claims);

            return claims.ToArray();
        }

        public override async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject,
            ResourceValidationResult resources,
            bool includeAllIdentityClaims,
            ValidatedRequest request)
        {
            var claims = (await base.GetIdentityTokenClaimsAsync(subject, resources, includeAllIdentityClaims, request)).ToList();
            await this.AppendClaimsAsync(subject, request, claims);
            return claims.ToArray();
        }
        /// <summary>
        /// 附加Claims
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="request"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        private async Task AppendClaimsAsync(ClaimsPrincipal subject,
                                             ValidatedRequest request,
                                             IList<Claim> claims)
        {
            var client = await this._mediator.SendAsync<ClientDto>(new Command.Find<ClientDto> { Id = request.ClientId });

            claims.Add(new Claim(Constants.Claim.AppId, client.AppId, Constants.Claim.CustomType));

            foreach (var claim in this.GetCustomClaim(subject))
            {
                if (!claims.Any(s => s.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase) &&
                    s.Value.Equals(claim.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    claims.Add(claim);
                }
            }
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
