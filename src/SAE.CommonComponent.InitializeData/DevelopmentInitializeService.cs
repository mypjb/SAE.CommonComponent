using SAE.CommonComponent.Application.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public class DevelopmentInitializeService : InitializeService
    {
        public DevelopmentInitializeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
