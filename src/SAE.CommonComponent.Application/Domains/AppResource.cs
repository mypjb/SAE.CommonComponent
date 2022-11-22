using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Application.Domains
{
    /// <summary>
    /// 系统下的资源
    /// </summary>
    public class AppResource : Document
    {
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        public AppResource()
        {

        }
        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <param name="command"></param>
        public AppResource(AppResourceCommand.Create command)
        {
            this.Apply<AppResourceEvent.Create>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 资源索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 资源名词
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 资源访问地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 资源请求谓词 (get、post、put...)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="command"></param>
        public void Change(AppResourceCommand.Change command)
        {
            this.Apply<AppResourceEvent.Change>(command);
        }
        /// <summary>
        /// 查询系统下是否存在同名资源
        /// </summary>
        /// <param name="provider"></param>
        public async Task NameExistAsync(Func<AppResource, Task<AppResource>> provider)
        {
            var appResource = await provider.Invoke(this);
            if (appResource == null)
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(AppResource)} name exist");
        }
    }
}
