using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Application.Abstract.Dtos
{
    public class EndpointDto
    {
        public IEnumerable<string> RedirectUris { get; set; }
        public IEnumerable<string> PostLogoutRedirectUris { get; set; }
        public string SignIn  { get; set; }
    }
}
