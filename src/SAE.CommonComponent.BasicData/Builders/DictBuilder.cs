using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.BasicData.Builders
{
    public class DictBuilder : IBuilder<IEnumerable<DictDto>>
    {
        private readonly IStorage _storage;
        private readonly DictDto _root;
        public DictBuilder(IStorage storage)
        {
            this._storage = storage;
        }
        public Task BuildAsync(IEnumerable<DictDto> models)
        {
            var parentIds = models.Select(s => s.ParentId).ToArray();
            var parentDtos = this._storage.AsQueryable<DictDto>()
                                         .Where(s => parentIds.Contains(s.Id))
                                         .ToList();

            foreach (var dto in models)
            {
                dto.Parent = parentDtos.FirstOrDefault(s => s.Id == dto.ParentId);
            }

            return Task.CompletedTask;
        }
    }
}
