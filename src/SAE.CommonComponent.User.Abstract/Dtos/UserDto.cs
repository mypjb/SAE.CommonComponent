using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent.User.Abstract.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public Status Status { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
