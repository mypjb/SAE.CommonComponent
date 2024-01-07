using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Domains
{
    /// <summary>
    /// 授权规则
    /// </summary>
    public class Rule : Document
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Rule()
        {

        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="command"></param>
        public Rule(RuleCommand.Create command)
        {
            this.Apply<RuleEvent.Create>(command, e =>
            {
                if (e.Id.IsNullOrWhiteSpace())
                {
                    e.Id = Utils.GenerateId();
                }

                e.CreateTime = DateTime.UtcNow;
            });
        }
        /// <summary>
        /// 标识
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }

        /// <summary>
        /// 左值
        /// </summary>
        /// <value></value>
        public string Left { get; set; }
        /// <summary>
        /// 符号
        /// </summary>
        /// <remarks>
        /// ：<![CDATA[>、<、>=、<=]]>、=、!=、regex...
        /// </remarks>
        public string Symbol { get; set; }
        /// <summary>
        /// 右值
        /// </summary>
        /// <remarks>
        /// 再某些时候不存在右值，比如!$left
        /// </remarks><value></value>
        public string Right { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 规则是否存在，如果存在则触发异常。
        /// </summary>
        /// <exception cref="SAEException"/>
        /// <param name="provider">提供一个可以用来查询的委托</param>
        /// <returns></returns>
        public async Task NameExist(Func<Rule, Task<Rule>> provider)
        {
            var rule = await provider.Invoke(this);
            if (rule == null || this.Id.Equals(rule.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(Rule)} name exist");
        }

        /// <summary>
        /// 更改角色信息
        /// </summary>
        /// <param name="command"></param>
        public void Change(RuleCommand.Change command) =>
            this.Apply<RuleEvent.Change>(command);

        /// <summary>
        /// 将规则转换为字符串形似的表达式
        /// </summary>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(this.Right) ?
                        $"{this.Symbol}{this.Left}" :
                        $"{this.Left}{this.Symbol}{this.Right}";
        }
    }
}