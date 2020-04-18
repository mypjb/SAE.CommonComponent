using System.Linq;
using System;
using System.Collections.Generic;
using SAE.CommonComponent.Application.Abstract.Commands;
using SAE.CommonComponent.Application.Abstract.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Application.Abstract.Domains
{
    public class App : Document
    {
        public App()
        {

        }
        public App(AppCreateCommand command)
        {
            this.Apply<AppCreateEvent>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
                e.Secret = Utils.GenerateId();
            });
        }

        /// <summary>
        /// app id
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// app name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// app secret
        /// </summary>
        /// <value></value>
        public string Secret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Urls { get; set; }
        /// <summary>
        /// auth scope
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Scopes { get; set; }

        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// app status
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }

        public void Change(AppChangeCommand command)
        {
            this.Apply<AppChangeEvent>(command);
        }

        public void RefreshSecret()
        {
            this.Apply(new AppRefreshSecretEvent
            {
                Secret = Utils.GenerateId()
            });
        }

        public void Reference(AppReferenceScopeCommand command)
        {
            if (!command.Scopes.Any()) return;

            var scopes = this.Scopes.ToList();
            scopes.RemoveAll(command.Scopes.Contains);
            this.Apply(new AppReferenceScopeEvent
            {
                Scopes = scopes
            });
        }
        public void CancelReference(AppCancelReferenceScopeCommand command)
        {
            if (!command.Scopes.Any()) return;

            var scopes = this.Scopes.ToList();
            scopes.AddRange(command.Scopes);
            this.Apply(new AppReferenceScopeEvent
            {
                Scopes = scopes.Distinct().ToArray()
            });
        }

        public void ChangeStatus(AppChangeStatusCommand command)
        {
            this.Apply<AppChangeStatusEvent>(command);
        }

        public void Remove()
        {
            this.ChangeStatus(new AppChangeStatusCommand
            {
                Id = this.Id,
                Status = Status.Delete
            });
        }
    }
}
