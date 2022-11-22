using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Events
{
    /// <summary>
    /// 系统所属资源
    /// </summary>
    public partial class AppResourceEvent
    {
        /// <summary>
        /// 创建
        /// </summary>
        public class Create : Change
        {
            /// <summary>
            /// 标识
            /// </summary>
            /// <value></value>
            public string Id { get; set; }
            /// <summary>
            /// 索引,具有唯一性，不可更改
            /// </summary>
            public int Index { get; set; }
            /// <summary>
            /// 系统标识
            /// </summary>
            /// <value></value>
            public string AppId { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            /// <value></value>
            public DateTime CreateTime { get; set; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public class Change : IEvent
        {
            /// <summary>
            /// 资源名词
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            /// <value></value>
            public string Description { get; set; }
            /// <summary>
            /// 资源访问地址
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// 资源访问谓词 (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }

    }
}