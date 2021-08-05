using System;

namespace SAE.CommonComponent
{
    public class Constants
    {
        public const string Scope = "api";
        public const string ClusterName = "Default";
        public const string AppId = "default:appid";
        public const string Development = nameof(Development);
        public const string Production = nameof(Production);
        public const string DefaultSeparator = "_";
        public class App
        {
            public const string AppSecretFileName = "appSecret.txt";
        }
        public class Config
        {
            public class OAuth
            {
                public const string Authority = "oauth:authority";
                public const string Scope = "oauth:scope";
                public const string AppId = "oauth:appId";
                public const string AppSecret = "oauth:appSecret";
                public const char ScopeSeparator = ' ';
            }
            public class BasicInfo
            {
                public const string Name = "basicInfo:name";
            }
            public class Url
            {
                public const string Host = "url:host";
                public const string SignIn = "url:signIn";
            }
            
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
            public const string AppId = "app_id";
        }

        public class Menu
        {
            public const string RootId = "menu_00000000000000000000000000000000";
            public const string RootName = "root";
        }

        public class Dict
        {
            public const string RootId = "dict_00000000000000000000000000000000";
            public const string RootName = "root";
        }
    }
}
