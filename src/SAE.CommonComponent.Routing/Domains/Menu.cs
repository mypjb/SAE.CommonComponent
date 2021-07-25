using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using SAE.CommonComponent.Routing.Commands;
using SAE.CommonComponent.Routing.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System.Linq;

namespace SAE.CommonComponent.Routing.Domains
{
    public class Menu : Document
    {
        public Menu()
        {
            this.ParentId = Constants.Menu.RootId;
            this.PermissionIds = Enumerable.Empty<string>().ToArray();
        }

        public Menu(MenuCommand.Create command)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Menu.RootId;
            }
            this.Apply<MenuEvent.Create>(command, e => e.Id = Utils.GenerateId());
        }

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

        /// <summary>
        ///  is hidden
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// menu permission ids
        /// </summary>
        public string[] PermissionIds { get; set; }

        public async Task Change(MenuCommand.Change command, Func<string, Task<Menu>> parentProvider, Func<Menu, Task<bool>> menuProvider)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Menu.RootId;
            }
            this.Apply<MenuEvent.Change>(command);
            await this.ParentExist(parentProvider);
            await this.NotExist(menuProvider);
        }

        public async Task ParentExist(Func<string, Task<Menu>> menuProvider)
        {
            if (this.IsRoot()) return;

            Assert.Build(await menuProvider.Invoke(this.ParentId))
                  .NotNull("parent not exist and not root node!");
        }

        public async Task NotExist(Func<Menu, Task<bool>> menuProvider)
        {
            Assert.Build(await menuProvider.Invoke(this))
                  .False("menu is exist!");
        }

        /// <summary>
        /// is root none
        /// </summary>
        /// <returns></returns>
        public bool IsRoot()
        {
            return this.ParentId == Constants.Menu.RootId;
        }

        /// <summary>
        /// Reference permission
        /// </summary>
        /// <param name="command"></param>
        public void ReferencePermission(MenuCommand.ReferencePermission command)
        {
            var permissionIds = (this.PermissionIds ?? new string[0] { }).Concat(command.PermissionIds)
                                                   .Distinct()
                                                   .ToArray();

            this.Apply(new MenuEvent.ReferencePermission
            {
                PermissionIds = permissionIds
            });
        }

        /// <summary>
        /// delete permission
        /// </summary>
        /// <param name="command"></param>
        public void DeletePermission(MenuCommand.DeletePermission command)
        {

            if (!command?.PermissionIds.Any() ?? false) return;

            var permissionIds = this.PermissionIds.ToList();

            permissionIds.RemoveAll(s => command.PermissionIds.Contains(s));

            this.Apply(new MenuEvent.ReferencePermission
            {
                PermissionIds = permissionIds.ToArray()
            });
        }
    }
}