using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;

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
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
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
        public async Task NameExistAsync(Func<AppCluster, Task<AppCluster>> provider)
        {
            var cluster = await provider.Invoke(this);
            if (cluster == null)
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(AppCluster)} name exist");
        }
    }
}
