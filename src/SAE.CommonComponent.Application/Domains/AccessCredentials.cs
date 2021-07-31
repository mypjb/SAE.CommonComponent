using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Linq;

namespace SAE.CommonComponent.Application.Abstract.Domains
{
    public class AccessCredentials : Document
    {
        public AccessCredentials()
        {
            this.Scopes = Enumerable.Empty<string>().ToArray();
        }
        public AccessCredentials(AccessCredentialsCommand.Create command) : this()
        {
            this.Apply<AccessCredentialsEvent.Create>(command, e =>
            {
                e.Id = command.Id ?? Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
                e.Secret = command.Secret ?? Utils.GenerateId();
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
        public Endpoint Endpoint { get; set; }
        /// <summary>
        /// auth scope
        /// </summary>
        /// <value></value>
        public string[] Scopes { get; set; }

        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// app status
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
        public void Change(AccessCredentialsCommand.Change command)
        {
            this.Apply<AccessCredentialsEvent.Change>(command);
        }

        public void RefreshSecret()
        {
            this.Apply(new AccessCredentialsEvent.RefreshSecret
            {
                Secret = Utils.GenerateId()
            });
        }
        
        public void ChangeStatus(AccessCredentialsCommand.ChangeStatus command)
        {
            this.Apply<AccessCredentialsEvent.ChangeStatus>(command);
        }

        public void Delete()
        {
            this.ChangeStatus(new AccessCredentialsCommand.ChangeStatus
            {
                Id = this.Id,
                Status = Status.Delete
            });
        }

        
        
    }
}
