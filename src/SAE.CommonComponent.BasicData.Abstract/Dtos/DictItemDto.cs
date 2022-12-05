using System;
using System.Collections.Generic;
namespace SAE.CommonComponent.BasicData.Dtos
{
    /// <summary>
    /// 字典子集
    /// </summary>
    public class DictItemDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 父级标识，为空则代表不存在父级
        /// </summary>
        /// <value></value>
        public string ParentId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        /// <value></value>
        public int Sort { get; set; }
        /// <summary>
        /// 子级
        /// </summary>
        /// <value></value>
        public IEnumerable<DictItemDto> Items { get; set; }
    }
}