using System.Collections.Generic;
using System;

namespace SAE.CommonComponent.Identity.Dtos
{
    public class Scopes
    {

        public Scopes()
        {
            this.Storge = new List<string>();
        }

        public IList<string> Storge
        {
            get; set;
        }

    }
}
