﻿using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ProjectEvent
    {
        public class Create : Change
        {
            public string SolutionId { get; set; }
            public DateTime CreateTime { get; set; }
            public string Id { get; set; }
        }
        public class Change : IEvent
        {
            public string Name { get; set; }
        }

        public class RelevanceConfig : IEvent
        {
            /// <summary>
            /// 
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// project id
            /// </summary>
            public string ProjectId { get; set; }
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
        }

        public class ConfigChangeAlias : IEvent
        {
            public string Alias { get; set; }
        }

    }
}
