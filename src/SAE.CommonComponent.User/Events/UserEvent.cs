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
    public class UserEvent
    {
        public class Register : IEvent
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Account Account { get; set; }
            public Status Status { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class ChangePassword : IEvent
        {
            public string Password { get; set; }
        }

        public class ChangeStatus:IEvent
        {
            public Status Status { get; set; }
        }
    }
}
