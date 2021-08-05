using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Linq;

namespace SAE.CommonComponent.Application.Abstract.Domains
{
    public class Client : Document
    {
        public Client()
        {
            this.Scopes = Enumerable.Empty<string>().ToArray();
        }
        public Client(ClientCommand.Create command) : this()
        {
            this.Apply<ClientEvent.Create>(command, e =>
            {
                e.Id = command.Id ?? Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
                e.Secret = command.Secret ?? Utils.GenerateId();
            });
        }

        /// <summary>
        /// client id
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        public string AppId { get; set; }
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
        public void Change(ClientCommand.Change command)
        {
            this.Apply<ClientEvent.Change>(command);
        }

        public void RefreshSecret()
        {
            this.Apply(new ClientEvent.RefreshSecret
            {
                Secret = Utils.GenerateId()
            });
        }
        
        public void ChangeStatus(ClientCommand.ChangeStatus command)
        {
            this.Apply<ClientEvent.ChangeStatus>(command);
        }

        public void Delete()
        {
            this.ChangeStatus(new ClientCommand.ChangeStatus
            {
                Id = this.Id,
                Status = Status.Delete
            });
        }

    }
}
