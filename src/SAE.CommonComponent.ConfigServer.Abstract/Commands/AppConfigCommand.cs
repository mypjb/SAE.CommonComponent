using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class AppConfigCommand
    {
        public class Publish
        {
            /// <summary>
            /// App Id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }

        public class Preview : Publish
        {
        }
       
        public class ReferenceConfig
        {
            public string AppId { get; set; }
            public string[] ConfigIds { get; set; }
        }

        public class Change
        {
            public string Id { get; set; }
            public string Alias { get; set; }
            /// <summary>
            /// private
            /// </summary>
            public bool Private { get; set; }
        }

        public class Query : Paging
        {
            /// <summary>
            /// App Id
            /// </summary>
            public string AppId { get; set; }
            /// <summary>
            /// 环境变量Id
            /// </summary>
            public string EnvironmentId { get; set; }
        }

    }
}
