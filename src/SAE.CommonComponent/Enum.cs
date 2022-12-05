using System;

namespace SAE.CommonComponent
{
    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 软删除
        /// </summary>
        Delete = -1,
        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 1
    }
    /// <summary>
    /// 访问方式
    /// </summary>
    public enum AccessMethod
    {
        /// <summary>
        /// Get
        /// </summary>
        Get = 0,
        /// <summary>
        /// Put
        /// </summary>
        Put = 1,
        /// <summary>
        /// Delete
        /// </summary>
        Delete = 2,
        /// <summary>
        /// Post
        /// </summary>
        Post = 3,
        /// <summary>
        /// Head
        /// </summary>
        Head = 4,
        /// <summary>
        /// Trace
        /// </summary>
        Trace = 5,
        /// <summary>
        /// Patch
        /// </summary>
        Patch = 6,
        /// <summary>
        /// Connect
        /// </summary>
        Connect = 7,
        /// <summary>
        /// Options
        /// </summary>
        Options = 8,
        /// <summary>
        /// Custom
        /// </summary>
        Custom = 9,
        /// <summary>
        /// None
        /// </summary>
        None = byte.MaxValue
    }
    /// <summary>
    /// 字典类型
    /// </summary>
    public enum DictType
    {
        /// <summary>
        /// 环境变量
        /// </summary>
        Environment,

        /// <summary>
        /// 区域
        /// </summary>
        Scope,
        /// <summary>
        /// 租户
        /// </summary>
        Tenant,
    }
}
