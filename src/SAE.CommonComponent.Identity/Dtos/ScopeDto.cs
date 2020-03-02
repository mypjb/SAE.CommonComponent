using System;
using IdentityServer4.Models;

namespace SAE.CommonComponent.Identity.Dtos
{
    public class ScopeDto
    {
        public string Name { get; set; }
        public string Display { get; set; }

        public static implicit operator ApiResource(ScopeDto dto)
        {
            return new ApiResource(dto.Name,dto.Display);
        }
    }
}
