using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Builders
{
    public class UserRoleBuilder : IBuilder<IEnumerable<UserRoleDto>>, IBuilder<UserRoleDto>
    {
        public const string DefaultRole = "角色处于不可用状态";
        private readonly IMediator _mediator;
        private readonly IStorage _storage;

        public UserRoleBuilder(IMediator mediator, IStorage storage)
        {
            this._mediator = mediator;
            this._storage = storage;
        }
        public Task Build(IEnumerable<UserRoleDto> userRoleDtos)
        {
            var roleIds= userRoleDtos.Select(s => s.RoleId).ToArray();

            var roleDtos= this._storage.AsQueryable<RoleDto>()
                              .Where(s => roleIds.Contains(s.Id));

            foreach (var userRoleDto in userRoleDtos)
            {
                userRoleDto.RoleName = roleDtos.FirstOrDefault(s => s.Id == userRoleDto.RoleId)?.Name ?? DefaultRole;
            }
            return Task.CompletedTask;
        }

        public Task Build(UserRoleDto model)
        {
            return this.Build(new[] { model });
        }
    }
}
