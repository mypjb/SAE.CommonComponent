using System;
using System.Linq;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Application.Abstract.Domains
{
    /// <summary>
    /// 客户端认证凭据
    /// </summary>
    public class Client : Document
    {
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        public Client()
        {
            this.Scopes = Enumerable.Empty<string>().ToArray();
        }
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <param name="command"></param>
        public Client(ClientCommand.Create command) : this()
        {
            this.Apply<ClientEvent.Create>(command, e =>
            {
                e.Id = command.ClientId.IsNullOrWhiteSpace() ? Utils.GenerateId() : command.ClientId;
                e.Secret = command.ClientSecret.IsNullOrWhiteSpace() ? Utils.GenerateId() : command.ClientSecret;
                e.CreateTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// 标识，也是唯一公钥
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        /// <value></value>
        public string Secret { get; set; }
        /// <summary>
        /// 端点
        /// </summary>
        /// <value></value>
        public ClientEndpoint Endpoint { get; set; }
        /// <summary>
        /// 授权域
        /// </summary>
        /// <value></value>
        public string[] Scopes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// client 状态
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="command"></param>
        public void Change(ClientCommand.Change command)
        {
            this.Apply<ClientEvent.Change>(command);
        }
        /// <summary>
        /// 重新生成私钥
        /// </summary>
        public void RefreshSecret()
        {
            this.Apply(new ClientEvent.RefreshSecret
            {
                Secret = Utils.GenerateId()
            });
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(ClientCommand.ChangeStatus command)
        {
            this.Apply<ClientEvent.ChangeStatus>(command);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            this.ChangeStatus(new ClientCommand.ChangeStatus
            {
                Id = this.Id,
                Status = Status.Delete
            });
        }
    }
}
