using System.Collections.Generic;
namespace SAE.CommonComponent.BasicData.Dtos
{
    public class DictItemDto
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int Type { get; set; }
        public IEnumerable<DictItemDto> Items { get; set; }
    }
}