using System;

namespace SAE.CommonComponent.Routing.Dtos
{
    public class MenuDto
    {
        public string Id { get; set; }
        /// <summary>
        /// menu name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// url path
        /// </summary>
        /// <value></value>
        public string Path { get; set; }
        /// <summary>
        /// parent id
        /// </summary>
        /// <value></value>
        public string ParentId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}