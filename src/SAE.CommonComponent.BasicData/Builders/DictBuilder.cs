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
            this._root = new DictDto
            {
                Id = Constants.Dict.RootId,
                Name = Constants.Dict.RootName
            };
        }
        public Task Build(IEnumerable<DictDto> models)
        {
            var parentIds = models.Select(s => s.ParentId).ToArray();
            var parentDtos = this._storage.AsQueryable<DictDto>()
                                         .Where(s => parentIds.Contains(s.Id))
                                         .ToList();
            parentDtos.Add(this._root);

            foreach (var dto in models)
            {
                dto.Parent = parentDtos.FirstOrDefault(s => s.Id == dto.ParentId);
            }

            return Task.CompletedTask;
        }
    }
}
