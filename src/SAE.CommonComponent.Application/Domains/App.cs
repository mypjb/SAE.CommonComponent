using SAE.CommonComponent.Application.Abstract.Events;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAE.CommonComponent.Application.Abstract.Domains
{
    public class App : Document
    {
        //public const string DefaultRedirectUri = "/signin-oidc";
        //public const string DefaultPostLogoutRedirectUris = "/signout-oidc";
        //public const string DefaultSignIn = "/account/login";
        public App()
        {
            this.Scopes = Enumerable.Empty<string>();
        }
        public App(AppCommand.Create command) : this()
        {
            this.Apply<AppEvent.Create>(command, e =>
            {
                e.Id = command.Id ?? Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
                e.Secret = command.Secret ?? Utils.GenerateId();
                //if (e.Endpoint.RedirectUris.IsNull()||!e.Endpoint.RedirectUris.Any())
                //{
                //    e.Endpoint.RedirectUris = new[] { $"{command.Host}{DefaultRedirectUri}" };
                //}

                //if (e.Endpoint.RedirectUris.IsNull() || !e.Endpoint.PostLogoutRedirectUris.Any())
                //{
                //    e.Endpoint.RedirectUris = new[] { $"{command.Host}{DefaultPostLogoutRedirectUris}" };
                //}

                //if (!e.Endpoint.SignIn.IsNullOrWhiteSpace())
                //{
                //    e.Endpoint.SignIn = $"{command.Host}{DefaultSignIn}";
                //}
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

        public void Change(AppCommand.Change command)
        {
            this.Apply<AppEvent.Change>(command);
        }

        public void RefreshSecret()
        {
            this.Apply(new AppEvent.RefreshSecret
            {
                Secret = Utils.GenerateId()
            });
        }

        public void Reference(AppCommand.ReferenceScope command)
        {
            if (!command.Scopes.Any()) return;

            if (this.Scopes == null)
            {
                this.Scopes = Enumerable.Empty<string>();
            }

            var scopes = this.Scopes.ToList();

            scopes.AddRange(command.Scopes);

            this.Apply(new AppEvent.ReferenceScope
            {
                Scopes = scopes.Distinct().ToList()
            });
        }
        public void CancelReference(AppCommand.CancelReferenceScope command)
        {
            if (!command.Scopes.Any()) return;

            var scopes = this.Scopes.ToList();
            scopes.AddRange(command.Scopes);
            this.Apply(new AppEvent.ReferenceScope
            {
                Scopes = scopes.Distinct().ToArray()
            });
        }

        public void ChangeStatus(AppCommand.ChangeStatus command)
        {
            this.Apply<AppEvent.ChangeStatus>(command);
        }

        public void Remove()
        {
            this.ChangeStatus(new AppCommand.ChangeStatus
            {
                Id = this.Id,
                Status = Status.Delete
            });
        }
    }
}
