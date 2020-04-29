using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Domains
{
    public class Solution:Document
    {
        public Solution()
        {

        }

        public Solution(SolutionCommand.Create command)
        {
            this.Apply<SolutionCreateEvent>(command, e =>
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
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        protected override string GetIdentity()
        {
            return this.Id;
        }

       
        public void Change(SolutionCommand.Change Command) => this.Apply<SolutionChangeEvent>(Command);
    }
}
