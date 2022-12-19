using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Domains
{
    /// <summary>
    /// 超级管理员,
    /// 拥有该对象说明<see cref="targetId"/>拥有<see cref="AppId"/>超级管理员权限。
    /// </summary>
    public class SuperAdmin : Document, IEvent
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public SuperAdmin()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public SuperAdmin(SuperAdminCommand.Create command)
        {
            this.Apply<SuperAdmin>(command, s =>
            {
                s.Id = $"{this.TargetId}{this.AppId}".ToMd5();
            });
        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 目标标识
        /// </summary>
        /// <value></value>
        public string TargetId { get; set; }
        /// <summary>
        /// 系统标识
        /// </summary>
        /// <value></value>
        public string AppId { get; set; }
    }
}