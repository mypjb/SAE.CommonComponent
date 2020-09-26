using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public interface IInitializeService
    {
        Task Initial();
        Task Application();
        Task Authorize();
        Task User();
        Task Routing();
        Task ConfigServer();
    }
}
