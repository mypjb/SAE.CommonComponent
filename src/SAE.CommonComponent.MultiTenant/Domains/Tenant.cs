using System;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.MultiTenant.Commands;
using SAE.CommonComponent.MultiTenant.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.MultiTenant.Domains
{
    /// <summary>
    /// 租户
    /// </summary>
    public class Tenant : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public Tenant()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public Tenant(TenantCommand.Create command)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Tree.RootId;
            }
            this.Apply<TenantEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.Now;
            });
        }

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 租户类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="command"></param>
        /// <param name="TenantProvider"></param>
        public async Task Change(TenantCommand.Change command, Func<Tenant, Task<bool>> TenantProvider)
        {
            this.Apply<TenantEvent.Change>(command);
            await this.NotExist(TenantProvider);
        }
        /// <summary>
        /// 校验是否存在该父级
        /// </summary>
        /// <param name="TenantProvider"></param>
        public async Task ParentExist(Func<string, Task<Tenant>> TenantProvider)
        {
            if (this.IsRoot()) return;
            var root = await TenantProvider.Invoke(this.ParentId);
            Assert.Build(root)
                  .NotNull("这个父级不存在或被删除");
        }
        /// <summary>
        /// 校验租户是否存在。
        /// </summary>
        /// <param name="TenantProvider"></param>
        public async Task NotExist(Func<Tenant, Task<bool>> TenantProvider)
        {
            Assert.Build(await TenantProvider.Invoke(this))
                  .False("租户已存在!");
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(TenantCommand.ChangeStatus command)
        {
            this.Apply<TenantEvent.ChangeStatus>(command);
        }

        /// <summary>
        /// 是否属于顶级节点
        /// </summary>
        /// <returns></returns>
        public bool IsRoot()
        {
            return this.ParentId == Constants.Tree.RootId;
        }

    }
}