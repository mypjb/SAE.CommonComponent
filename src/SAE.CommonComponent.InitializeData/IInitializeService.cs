using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public interface IInitializeService
    {
        Task InitialAsync();
        Task BasicDataAsync();
        Task ApplicationAsync();
        Task AuthorizeAsync();
        Task UserAsync();
        Task RoutingAsync();
        Task ConfigServerAsync();
        Task PluginAsync();
    }
}
