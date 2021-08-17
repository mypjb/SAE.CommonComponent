using System;

namespace SAE.CommonComponent.BasicData.Dtos
{
    public class DictDto
    {
        public string Id { get; set; }
        /// <summary>
        /// Dict name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DictDto Parent { get; set; }

        /// <summary>
        /// Parent id
        /// </summary>
        /// <value></value>
        public string ParentId { get; set; }

        /// <summary>
        /// Dict type
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
