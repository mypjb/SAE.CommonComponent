using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.BasicData.Domains
{
    public class Dict : Document
    {
        public const string DefaultId = "dict_00000000000000000000000000000000";
        public Dict()
        {
            this.ParentId = DefaultId;
        }

        public Dict(DictCommand.Create command)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = DefaultId;
            }
            this.Apply<DictEvent.Create>(command, e => e.Id = Utils.GenerateId());
        }

        public string Id { get; set; }
        /// <summary>
        /// Dict name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// dict type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// parent id
        /// </summary>
        /// <value></value>
        public string ParentId { get; set; }

        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }

        public async Task Change(DictCommand.Change command, Func<string, Task<Dict>> parentProvider, Func<Dict, Task<bool>> DictProvider)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = DefaultId;
            }
            this.Apply<DictEvent.Change>(command);
            await this.ParentExist(parentProvider);
            await this.NotExist(DictProvider);
        }

        public async Task ParentExist(Func<string, Task<Dict>> DictProvider)
        {
            if (this.IsRoot()) return;
            var root = await DictProvider.Invoke(this.ParentId);
            Assert.Build(root)
                  .NotNull("parent not exist and not root node!");

            Assert.Build(this.Type == root.Type)
                  .True($"type not equal parent");
        }

        public async Task NotExist(Func<Dict, Task<bool>> DictProvider)
        {
            Assert.Build(await DictProvider.Invoke(this))
                  .False("Dict is exist!");
        }

        /// <summary>
        /// is root none
        /// </summary>
        /// <returns></returns>
        public bool IsRoot()
        {
            return this.ParentId == DefaultId;
        }

    }
}