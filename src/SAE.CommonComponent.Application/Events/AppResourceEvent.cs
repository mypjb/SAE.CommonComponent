using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Events
{
    public partial class AppResourceEvent
    {
        public class Create : Change
        {
            public string Id { get; set; }
            public string AppId { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class Change : IEvent
        {
            /// <summary>
            /// resource index relative to the app
            /// </summary>
            public int Index { get; set; }
            /// <summary>
            /// resource name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// resource path
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// resource method (get、post、put...)
            /// </summary>
            public string Method { get; }
        }

    }
}