using System;

namespace SAE.CommonComponent
{
    public class Constants
    {
        public const string Scope = "api";
        public const string SolutionName = "Default";
        public const string Development = nameof(Development);
        public const string Production = nameof(Production);
        public const string DefaultSeparator = "_";
        public class Config
        {
            public class OAuth
            {
                public const string Authority = "authority";
                public const string Scope = "scope";
                public const string AppId = "appId";
                public const string AppSecret = "appSecret";
                public const char ScopeSeparator = ' ';
            }

            public const string AppName = "appName";
            public const string Master = "master";
            public const string ConfigExtensionName = ".json";
            public const char Separator = '.';
        }

        public class User
        {
            public const string Name = "admin";
            public const string Password = "admin";
        }

        public class Claim
        {
            public const string CustomType = "0";
            public const string ClientId = "client_id";
        }

        public class Menu
        {
            public const string DefaultId = "menu_00000000000000000000000000000000";
        }

        public class Dict
        {
            public const string DefaultId = "dict_00000000000000000000000000000000";
        }
    }
}
