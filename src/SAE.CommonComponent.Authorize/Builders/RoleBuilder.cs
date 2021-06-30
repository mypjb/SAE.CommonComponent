using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Builders
{
    public class RoleBuilder : IBuilder<IEnumerable<RoleDto>>, IBuilder<RoleDto>
    {
        private readonly IMediator _mediator;
        private readonly IStorage _storage;

        public RoleBuilder(IMediator mediator, IStorage storage)
        {
            this._mediator = mediator;
            this._storage = storage;
        }
        public async Task Build(IEnumerable<RoleDto> models)
        {
            var permissionDtos = await this._mediator.SendAsync<IEnumerable<PermissionDto>>(new Command.List<PermissionDto>());

            foreach (RoleDto dto in models)
            {
                if (dto.PermissionIds != null && dto.PermissionIds.Any())
                {
                    dto.Permissions = permissionDtos.Where(s => dto.PermissionIds.Contains(s.Id)).ToArray();
                }
            }
        }

        public Task Build(RoleDto model)
        {
            return this.Build(new[] { model });
        }
    }
}
