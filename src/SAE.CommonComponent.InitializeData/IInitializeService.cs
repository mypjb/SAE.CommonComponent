using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SAE.CommonComponent.InitializeData
{
    public interface IInitializeService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="app"></param>
        Task InitialAsync(IApplicationBuilder app);
        /// <summary>
        /// 基础数据
        /// </summary>
        Task BasicDataAsync();
        /// <summary>
        /// 系统
        /// </summary>
        Task ApplicationAsync();
        /// <summary>
        /// 授权
        /// </summary>
        Task AuthorizeAsync();
        /// <summary>
        /// 多租户
        /// </summary>
        Task MultiTenantAsync();
        /// <summary>
        /// 用户
        /// </summary>
        Task UserAsync();
        /// <summary>
        /// 路由
        /// </summary>
        Task RoutingAsync();
        /// <summary>
        /// 配置服务
        /// </summary>
        Task ConfigServerAsync();
        /// <summary>
        /// 插件
        /// </summary>
        Task PluginAsync();
    }
}
