using System.Collections.Generic;
namespace SAE.CommonComponent.Routing.Dtos
{
    public class MenuItemDto
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string Path { get; set; }
        public string ParentId { get; set; }
        public IEnumerable<MenuItemDto> Items { get; set; }
    }
}