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
    /// 应用集群
    /// </summary>
    public class AppCluster : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public AppCluster()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public AppCluster(AppClusterCommand.Create command)
        {
            this.Apply<AppClusterEvent.Create>(command, e =>
            {
                if (command.Id.IsNullOrWhiteSpace())
                {
                    e.Id = Utils.GenerateId();
                }
                else
                {
                    e.Id = command.Id;
                }
                e.CreateTime = DateTime.UtcNow;
                e.Status = Status.Enable;
            });
        }

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 集群名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 集群类型。（集群下的资源属于多租户时，应该设置该值，该值对应字典标识，一旦设置不可更改!)
        /// </summary>
        /// <value></value>
        public string Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 集群设置，集群设置(当集群类型存在时，应该设置该对象)
        /// </summary>
        /// <value></value>
        public AppClusterSetting Setting { get; set; }
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
        public void Change(AppClusterCommand.Change command)
        {
            this.Apply<AppClusterEvent.Change>(command);
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(AppClusterCommand.ChangeStatus command)
        {
            this.Apply<AppClusterEvent.ChangeStatus>(command);
        }
        /// <summary>
        /// 是否存在重名集群
        /// </summary>
        /// <param name="provider"></param>
        public async Task ExistAsync(Func<AppCluster, Task<AppCluster>> provider)
        {
            var cluster = await provider.Invoke(this);
            if (cluster == null)
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(AppCluster)} exist");
        }
    }
}
