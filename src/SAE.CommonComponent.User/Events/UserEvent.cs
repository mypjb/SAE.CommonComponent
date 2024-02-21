using SAE.CommonComponent.User.Domains;
using SAE.CommonLibrary.EventStore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.User.Events
{
    /// <summary>
    /// 用户事件
    /// </summary>
    public class UserEvent
    {
        /// <summary>
        /// 注册
        /// </summary>
        public class Register : IEvent
        {
            /// <summary>
            /// 标识
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 昵称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 账号
            /// </summary>
            public Account Account { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public Status Status { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        public class ChangePassword : IEvent
        {
            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }
        }
        /// <summary>
        /// 变更状态
        /// </summary>
        public class ChangeStatus:IEvent
        {
            /// <summary>
            /// 状态
            /// </summary>
            public Status Status { get; set; }
        }
    }
}
