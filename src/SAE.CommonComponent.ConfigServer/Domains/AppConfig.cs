using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Commands;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    public class AppConfig : Document
    {
        public AppConfig()
        {

        }

        public AppConfig(string appId, Config config)
        {

            this.Apply(new AppConfigEvent.ReferenceConfig
            {
                Id = $"{appId}{Constants.DefaultSeparator}{config.Id}{Constants.DefaultSeparator}{config.EnvironmentId}",
                AppId = appId,
                ConfigId = config.Id,
                Alias = config.Name,
                EnvironmentId = config.EnvironmentId,
                Private = true
            });
        }

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// App Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 配置Id
        /// </summary>
        public string ConfigId { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public string EnvironmentId { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// private
        /// </summary>
        public bool Private { get; set; }

        protected override string GetIdentity()
        {
            return this.Id;
        }

        internal void Change(AppConfigCommand.Change command)
        {
            this.Apply<AppConfigEvent.ConfigChange>(command);
        }
    }
}
