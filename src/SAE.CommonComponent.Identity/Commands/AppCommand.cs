using System.Collections;
using System.Collections.Generic;
using SAE.CommonComponent.Identity.Domains;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Identity.Commands
{
    public class AppQueryCommand : Paging
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
    public class AppCreateCommand
    {
        public string Name { get; set; }
        public IEnumerable<string> Urls { get; set; }
    }
    public class AppChangeCommand : AppCreateCommand
    {
        public string Id { get; set; }
    }
    public class AppRefreshSecretCommand
    {
        public string Id { get; set; }
    }

    public class AppReferenceScopeCommand
    {
        public string Id { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }

    public class AppCancelReferenceScopeCommand
    {
        public string Id { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }

    public class AppChangeStatusCommand
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }
}