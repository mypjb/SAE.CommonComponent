namespace SAE.CommonComponent.Application.Dtos
{

    /// <summary>
    /// 集群设置(当集群类型存在时，应该设置该对象)
    /// </summary>
    public class AppClusterSettingDto
    {
        /// <summary>
        /// 对应多租户的策略（基于域名、用户标识、请求头...）
        /// </summary>
        /// <value></value>
        public int Strategy { get; set; }
        /// <summary>
        /// 用户标识名称
        /// </summary>
        /// <value></value>
        public string ClaimName
        {
            get; set;
        }

        /// <summary>
        /// 请求头名称
        /// </summary>
        /// <value></value>
        public string HeaderName
        {
            get; set;
        }


        /// <summary>
        /// 主域
        /// </summary>
        /// <value></value>
        public string Host { get; set; }

        /// <summary>
        /// 使用默认策略
        /// </summary>
        /// <value></value>
        public bool UseDefaultRule { get; set; }
    }
}
