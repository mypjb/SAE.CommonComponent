using System;
using System.Collections.Generic;
using System.Text;

namespace SAE.CommonComponent.ConfigServer.Test.Models
{
    public class CustomConfig
    {
        public BasicConifg Basic { get; set; }
        public DateTime Create { get; set; }
    }

    public class Contact
    {
        public string QQ { get; set; }
        public string Wechat { get; set; }
        public string Tel { get; set; }
    }

    public class BasicConifg
    {
        public string Name { get; set; }
        public Contact Contact { get; set; }
    }
}
