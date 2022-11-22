using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Application.Domains
{
    /// <summary>
    /// 集群下的应用
    /// </summary>
    public class App : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public App()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public App(AppCommand.Create command)
        {
            this.Apply<AppEvent.Create>(command, e =>
            {
                if (e.Id.IsNullOrWhiteSpace())
                {
                    e.Id = Utils.GenerateId();
                }

                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// 系统标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 集群id
        /// </summary>
        public string ClusterId { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="command"></param>
        public void Change(AppCommand.Change command)
        {
            this.Apply<AppEvent.Change>(command);
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(AppCommand.ChangeStatus command)
        {
            this.Apply<AppEvent.ChangeStatus>(command);
        }
        /// <summary>
        /// 在同一个集群下是否存在同名系统
        /// </summary>
        /// <param name="provider"></param>
        public async Task NameExistAsync(Func<App, Task<App>> provider)
        {
            var app = await provider.Invoke(this);
            if (app == null)
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(App)} name exist");
        }
    }
}
