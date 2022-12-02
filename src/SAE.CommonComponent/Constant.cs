using System;

namespace SAE.CommonComponent
{
    /// <summary>
    /// 常量集合
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 域
        /// </summary>
        public const string Scope = "api";
        /// <summary>
        /// 默认集群名称
        /// </summary>
        public const string ClusterName = "Default";
        /// <summary>
        /// 开发环境标识符
        /// </summary>
        public const string Development = nameof(Development);
        /// <summary>
        /// 生产环境标识符
        /// </summary>
        public const string Production = nameof(Production);
        /// <summary>
        /// 默认分割负
        /// </summary>
        public const string DefaultSeparator = "_";
        /// <summary>
        /// 应用常量
        /// </summary>
        public class App
        {
            /// <summary>
            /// 公钥文件名称
            /// </summary>
            public const string AppSecretFileName = "appSecret.txt";
        }
        /// <summary>
        /// 授权常量
        /// </summary>
        public class Authorize
        {
            /// <summary>
            /// 权限格式化字符串
            /// </summary>
            /// <value></value>
            public const string PermissionFormat = "{0}:{1}";
            /// <summary>
            /// 超管名称
            /// </summary>
            public const string AdminRoleName = "admin";
        }
        /// <summary>
        /// 配置节
        /// </summary>
        public class Config
        {
            /// <summary>
            /// 认证
            /// </summary>
            public class OAuth
            {
                /// <summary>
                /// 认证地址
                /// </summary>
                public const string Authority = "oauth:authority";
                /// <summary>
                /// 作用范围
                /// </summary>
                public const string Scope = "oauth:scope";
                /// <summary>
                /// 公钥
                /// </summary>
                public const string AppId = "oauth:appId";
                /// <summary>
                /// 私钥
                /// </summary>
                public const string AppSecret = "oauth:appSecret";
                /// <summary>
                /// 区域分割符
                /// </summary>
                public const char ScopeSeparator = ' ';
            }
            /// <summary>
            /// 基础配置
            /// </summary>
            public class BasicInfo
            {
                /// <summary>
                /// 名称
                /// </summary>
                public const string Name = "basicInfo:name";
            }
            /// <summary>
            /// 地址
            /// </summary>
            public class Url
            {
                /// <summary>
                /// 域名
                /// </summary>
                public const string Host = "url:host";
                /// <summary>
                /// 登陆地址
                /// </summary>
                public const string SignIn = "url:signIn";
            }
            /// <summary>
            /// 主配置
            /// </summary>
            public const string Master = "master";
            /// <summary>
            /// 配置文件后缀
            /// </summary>
            public const string ConfigExtensionName = ".json";
            /// <summary>
            /// 分割符
            /// </summary>
            public const char Separator = '.';
        }
        /// <summary>
        /// 用户常量
        /// </summary>
        public class User
        {
            /// <summary>
            /// 默认名称
            /// </summary>
            public const string Name = "admin";
            /// <summary>
            /// 默认密码
            /// </summary>
            public const string Password = "admin";
        }
        /// <summary>
        /// 声明类型
        /// </summary>
        public class Claim
        {
            /// <summary>
            /// 自定义类型
            /// </summary>
            public const string CustomType = "0";
            /// <summary>
            /// 客户端标识
            /// </summary>
            public const string ClientId = "client_id";
            /// <summary>
            /// 应用标识
            /// </summary>
            public const string AppId = "app_id";
        }
        /// <summary>
        /// 菜单
        /// </summary>
        public class Menu
        {
            /// <summary>
            /// 根标识
            /// </summary>
            public const string RootId = "menu_0";
            /// <summary>
            /// 根名称
            /// </summary>
            public const string RootName = "root";
        }
        /// <summary>
        /// 字典
        /// </summary>
        public class Dict
        {
            /// <summary>
            /// 根标识
            /// </summary>
            public const string RootId = "dict_0";
            /// <summary>
            /// 根名称
            /// </summary>
            public const string RootName = "root";
        }
        /// <summary>
        /// 租户
        /// </summary>
        public class Tenant
        {
            /// <summary>
            /// 根标识
            /// </summary>
            public const string RootId = "tenant_0";
            /// <summary>
            /// 根名称
            /// </summary>
            public const string RootName = "root";
        }
    }
}
