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

namespace SAE.CommonComponent.Routing.Domains
{
    public class Menu : Document
    {
        public const string DefaultId = "00000000000000000000000000000000";
        public Menu()
        {
            this.ParentId = DefaultId;
        }

        public Menu(MenuCommand.Create command)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = DefaultId;
            }
            this.Apply<MenuCreateEvent>(command, e => e.Id = Utils.GenerateId());
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
        public DateTime CreateTime { get; set; }

        public async Task Change(MenuCommand.Change command, Func<string, Task<Menu>> parentProvider, Func<Menu, Task<bool>> menuProvider)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = DefaultId;
            }
            this.Apply<MenuChangeEvent>(command);
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

        public bool IsRoot()
        {
            return this.ParentId == DefaultId;
        }
    }
}