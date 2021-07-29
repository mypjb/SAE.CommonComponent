using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Builders
{
    public class ProjectBuilder : IBuilder<IEnumerable<ProjectConfigDto>>, IBuilder<IEnumerable<ProjectDetailDto>>
    {
        private readonly IStorage _storage;

        public ProjectBuilder(IStorage storage)
        {
            this._storage = storage;
        }


        public async Task Build(IEnumerable<ProjectConfigDto> dtos)
        {
            var ids = dtos.Select(s => s.ConfigId);

            var configDtos = this._storage.AsQueryable<ConfigDto>().Where(s => ids.Contains(s.Id));

            foreach (var dto in dtos)
            {
                dto.Config = configDtos.FirstOrDefault(s => s.Id == dto.ConfigId);
            }
        }

        public Task Build(IEnumerable<ProjectDetailDto> dtos)
        {
            var ids= dtos.Select(s => s.SolutionId);

            var solutionDtos= this._storage.AsQueryable<SolutionDto>().Where(s => ids.Contains(s.Id));

            foreach (var dto in dtos)
            {
                dto.SolutionName = solutionDtos.FirstOrDefault(s => s.Id == dto.SolutionId)?.Name;
            }

            return Task.CompletedTask;
        }
    }
}
