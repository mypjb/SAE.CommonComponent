﻿using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Handlers
{
    public class RoleHandler : AbstractHandler<Role>,
                              ICommandHandler<RoleCommand.Create, string>,
                              ICommandHandler<RoleCommand.Change>,
                              ICommandHandler<RoleCommand.ChangeStatus>,
                              ICommandHandler<RoleCommand.RelevancePermission>,
                              ICommandHandler<RoleCommand.DeletePermission>,
                              ICommandHandler<Command.BatchDelete<Role>>,
                              ICommandHandler<Command.Find<RoleDto>,RoleDto>,
                              ICommandHandler<RoleCommand.Query, IPagedList<RoleDto>>

    {
        private readonly IStorage _storage;
        private readonly IDirector _director;

        public RoleHandler(IDocumentStore documentStore, 
                          IStorage storage,
                          IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._director = director;
        }

        public async Task<string> HandleAsync(RoleCommand.Create command)
        {
            var role = new Role(command);
            await role.NameExist(this.FindRole);
            await this.AddAsync(role);
            return role.Id;
        }

        public async Task HandleAsync(RoleCommand.Change command)
        {
            await this.UpdateAsync(command.Id,async role =>
            {
                role.Change(command);
                await role.NameExist(this.FindRole);
            });
        }

        public async Task HandleAsync(RoleCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, role =>
            {
                role.ChangeStatus(command);
            });
        }

        public async Task HandleAsync(Command.BatchDelete<Role> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                var role= await this._documentStore.FindAsync<Role>(id);
                role.Delete();
                await this._documentStore.SaveAsync(role);
            });
        }

        public async Task<IPagedList<RoleDto>> HandleAsync(RoleCommand.Query command)
        {
            var query = this._storage.AsQueryable<RoleDto>()
                                     .Where(s => s.Status > Status.Delete);
            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name));

            var dtos = PagedList.Build(query, command);

            await this._director.Build(dtos.AsEnumerable());

            return dtos;
        }

        public async Task<RoleDto> HandleAsync(Command.Find<RoleDto> command)
        {
            var dto= this._storage.AsQueryable<RoleDto>()
                                  .FirstOrDefault(s => s.Id == command.Id&& s.Status!= Status.Delete);
            return dto;
        }

        public async Task HandleAsync(RoleCommand.RelevancePermission command)
        {
            var role =await this.FindAsync(command.Id);

            Assert.Build(role)
                  .IsNotNull();

            role.RelationPermission(command);

            await this._documentStore.SaveAsync(role);
        }

        public async Task HandleAsync(RoleCommand.DeletePermission command)
        {
            var role = await this.FindAsync(command.Id);

            Assert.Build(role)
                  .IsNotNull();

            role.DeletePermission(command);

            await this._documentStore.SaveAsync(role);
        }

        private Task<string> FindRole(string name)
        {
            var dto = this._storage.AsQueryable<RoleDto>()
                                   .FirstOrDefault(s => s.Name.Contains(name));
            return Task.FromResult(dto?.Name ?? string.Empty);
        }
    }
}