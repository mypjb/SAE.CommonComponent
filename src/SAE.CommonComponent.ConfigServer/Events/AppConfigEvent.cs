using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class AppConfigEvent
    {
        public class ReferenceConfig : IEvent
        {
            /// <summary>
            /// 
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// App Id
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// config id
            /// </summary>
            public string ConfigId { get; set; }
            /// <summary>
            /// alias 
            /// </summary>
            public string Alias { get; set; }
            /// <summary>
            /// env id
            /// </summary>
            public string EnvironmentId { get; set; }
            public bool Private { get; set; }
        }

        public class ConfigChange : IEvent
        {
            public string Alias { get; set; }
            public bool Private { get; set; }
        }

    }
}
