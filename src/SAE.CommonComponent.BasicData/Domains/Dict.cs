using System;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.BasicData.Domains
{
    /// <summary>
    /// 字典
    /// </summary>
    public class Dict : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public Dict()
        {
            this.ParentId = Constants.Tree.RootId;
        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public Dict(DictCommand.Create command)
        {
            this.Apply<DictEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.Now;
                if (e.ParentId.IsNullOrWhiteSpace())
                {
                    e.ParentId = Constants.Tree.RootId;
                }
            });

        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        /// <value></value>
        public int Sort { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        /// <value></value>
        public string ParentId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更改对象
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parentProvider"></param>
        /// <param name="dictProvider"></param>
        public async Task Change(DictCommand.Change command, Func<string, Task<Dict>> parentProvider, Func<Dict, Task<bool>> dictProvider)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Tree.RootId;

            }
            this.Apply<DictEvent.Change>(command);

            await this.ParentExist(parentProvider);
            await this.NotExist(dictProvider);

        }
        /// <summary>
        /// 父级是否存在
        /// </summary>
        /// <param name="provider"></param>
        public async Task ParentExist(Func<string, Task<Dict>> provider)
        {
            if (this.ParentId == Constants.Tree.RootId)
            {
                return;
            }

            var root = await provider.Invoke(this.ParentId);
            Assert.Build(root)
                  .NotNull("不存在父级节点");

        }
        /// <summary>
        /// 字典不存在
        /// </summary>
        /// <param name="provider"></param>
        public async Task NotExist(Func<Dict, Task<bool>> provider)
        {
            Assert.Build(await provider.Invoke(this))
                  .False("字典不存在！");
        }
    }
}