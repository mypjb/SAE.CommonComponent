using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Builders
{
    /// <summary>
    /// 角色构建者
    /// </summary>
    /// <inheritdoc/>
    public class RoleBuilder : IBuilder<IEnumerable<RoleDto>>, IBuilder<RoleDto>
    {
        private readonly IMediator _mediator;
        private readonly IStorage _storage;
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="storage"></param>
        public RoleBuilder(IMediator mediator, IStorage storage)
        {
            this._mediator = mediator;
            this._storage = storage;
        }

        public async Task Build(IEnumerable<RoleDto> models)
        {
            var pids = new List<string>();
            models.ForEach(s =>
            {
                if (s.PermissionIds != null)
                    pids.AddRange(s.PermissionIds);
            });

            pids = pids.Distinct().ToList();

            if (!pids.Any())
            {
                return;
            }

            var permissionDtos = await this._mediator.SendAsync<IEnumerable<PermissionDto>>(new PermissionCommand.Finds { Ids = pids.ToArray() });

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
