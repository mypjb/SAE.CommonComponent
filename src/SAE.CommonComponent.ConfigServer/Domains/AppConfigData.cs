using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    /// <summary>
    /// 应用配置数据
    /// </summary>
    public class AppConfigData:Document
    {
        /// <summary>
        /// ctor
        /// </summary>
        public AppConfigData()
        {

        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="event"></param>
        public AppConfigData(AppConfigDataEvent.Create @event)
        {
            this.Apply<AppConfigDataEvent.Create>(@event,e=>
            {
                e.Id = $"{@event.AppId}{Constants.DefaultSeparator}{@event.EnvironmentId}".ToMd5();
                e.Version = 1;
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
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 环境Id
        /// </summary>
        public string EnvironmentId { get; set; }
        /// <summary>
        /// 项目已发布的配置数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 公共配置
        /// </summary>
        public string PublicData { get; set; }
        /// <summary>
        /// 获得标识
        /// </summary>
        /// <returns></returns>
        protected override string GetIdentity()
        {
            return this.Id;
        }

        public void Change(AppConfigDataEvent.Publish @event)
        {
            this.Apply<AppConfigDataEvent.Publish>(@event, e =>
            {
                e.Version = this.Version + 1;
            });
        }
    }
}
