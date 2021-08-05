using SAE.CommonLibrary.EventStore;
using System;

namespace SAE.CommonComponent.ConfigServer.Events
{
    public class ConfigEvent
    {
        public class Create : Change
        {
            /// <summary>
            /// Cluster Id
            /// </summary>
            public string ClusterId { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string EnvironmentId { get; set; }
            public string Id { get; set; }
        }

        public class Change : IEvent
        {
            /// <summary>
            /// 模板类型
            /// </summary>
            public string TemplateId { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }

            public int Version { get; set; }

        }
    }
}
