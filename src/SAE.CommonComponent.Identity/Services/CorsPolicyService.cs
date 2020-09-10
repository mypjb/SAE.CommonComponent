using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Identity.Services
{
    public class CorsPolicyService : DefaultCorsPolicyService, ICorsPolicyService
    {
        public CorsPolicyService(ILogger<DefaultCorsPolicyService> logger) : base(logger)
        {
            this.AllowAll = true;
        }
    }
}
