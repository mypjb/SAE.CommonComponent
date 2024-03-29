﻿using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config : Document
    {
        public Config()
        {

        }
        public Config(ConfigCommand.Create command)
        {
            this.Apply<ConfigEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Cluster Id
        /// </summary>
        public string ClusterId { get; set; }
        /// <summary>
        /// environment
        /// </summary>
        public string EnvironmentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// version
        /// </summary>
        /// <value></value>
        public int Version { get; set; }
        public void Change(ConfigCommand.Change command)
        {
            this.Apply<ConfigEvent.Change>(command, e => e.Version = this.Version + 1);
        }
    }
}
