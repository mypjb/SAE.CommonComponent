using System;

namespace SAE.CommonComponent
{
    public class Constants
    {
        public const string Scope = "api";
        
        public class Production
        {
            public const string AppId = "localhost.test";
            public const string Secret = "localhost.test";
            public const string AppName = "master";
            public const string Master = "http://master.client.sae.com";
            public const string Authority = "http://identity.sae.com";
        }
        public class Development
        {
            public const string AppId = "localhost.test";
            public const string Secret = "localhost.test";
            public const string AppName = "master";
            public const string Master = "http://localhost:8000";
            public const string Authority = "http://localhost:8080";
        }
    }
}
