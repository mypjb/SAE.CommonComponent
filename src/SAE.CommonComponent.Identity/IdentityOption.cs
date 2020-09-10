using System;

namespace SAE.CommonComponent.Identity
{
    public class IdentityOption
    {
        public IdentityOption()
        {
            this.AuthorizationCodeLifetime = 60 * 1;
        }
        public int AuthorizationCodeLifetime { get; set; }
    }
}
