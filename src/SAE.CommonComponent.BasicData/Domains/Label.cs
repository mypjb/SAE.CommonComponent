using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.BasicData.Domains
{
    /// <summary>
    /// 标签
    /// </summary>
    public class Label : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public Label()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public Label(LabelCommand.Create command)
        {
            this.Apply<LabelEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.Now;
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
        /// 值
        /// </summary>
        /// <value></value>
        public string Value { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        /// <value></value>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        public DateTime CreateTime { get; set; }

    }
}