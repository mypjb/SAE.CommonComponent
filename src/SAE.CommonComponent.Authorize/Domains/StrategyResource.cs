using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Domains
{
    /// <summary>
    /// 策略资源
    /// </summary>
    /// <remarks>
    /// 描述资源如何与策略进行关联，该对象只允许创建和删除不允许修改。
    /// </remarks>
    public class StrategyResource : Document
    {
        /// <summary>
        /// 
        /// </summary>
        public StrategyResource()
        {

        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="command">创建命令</param>
        public StrategyResource(StrategyResourceCommand.Create command)
        {
            this.Apply<StrategyResourceEvent.Create>(command, e =>
            {
                e.Id = $"{command.StrategyId}_{command.ResourceType}_{command.ResourceId}".ToMd5().ToLower();
                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 资源类型，和字典相互关联
        /// </summary>
        /// <value></value>
        public string ResourceType { get; set; }
        /// <summary>
        /// 资源标识
        /// </summary>
        /// <value></value>
        public string ResourceId { get; set; }
        /// <summary>
        /// 策略标识
        /// </summary>
        /// <value></value>
        public string StrategyId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        public DateTime CreateTime { get; set; }
    }
}