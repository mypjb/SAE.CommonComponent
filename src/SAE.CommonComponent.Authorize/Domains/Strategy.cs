using System;
using System.Threading.Tasks;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Authorization.ABAC;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonComponent.Authorize.Events;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonLibrary.Extension;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SAE.CommonComponent.Authorize.Domains
{
    /// <summary>
    /// 策略
    /// </summary>
    public class Strategy : Document
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Strategy()
        {

        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="command"></param>
       public Strategy(StrategyCommand.Create command)
        {
            this.Apply<StrategyEvent.Create>(command, e =>
                        {
                            if (e.Id.IsNullOrWhiteSpace())
                            {
                                e.Id = Utils.GenerateId();
                            }

                            e.CreateTime = DateTime.UtcNow;
                            e.Status = Status.Disable;
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
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// 规则组合
        /// </summary> 
        public RuleCombine RuleCombine { get; set; }
        /// <summary>
        /// 策略的表达式，通过调用<see cref="BuildAsync(Func{string, Task{Rule}})"/>，进行赋值
        /// </summary>
        public string Expression { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 策略是否存在，如果存在则触发异常。
        /// </summary>
        /// <exception cref="SAEException"/>
        /// <param name="provider">提供一个可以用来查询的委托</param>
        /// <returns></returns>
        public async Task NameExist(Func<Strategy, Task<Strategy>> provider)
        {
            var strategy = await provider.Invoke(this);
            if (strategy == null || this.Id.Equals(strategy.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            throw new SAEException(StatusCodes.ResourcesExist, $"{nameof(strategy)} name exist");
        }

        /// <summary>
        /// 构建策略的表达式
        /// </summary>
        /// <param name="provider">rule存储委托</param>
        public async Task BuildAsync(Func<string, Task<Rule>> provider)
        {
            var expression = string.Empty;
            if (this.RuleCombine != null)
            {
                expression = await this.RuleCombine.CombineAsync(provider);
            }

            this.Apply(new StrategyEvent.Build
            {
                Expression = expression
            });
        }

        /// <summary>
        /// 更改角色信息
        /// </summary>
        /// <param name="command"></param>
        public void Change(StrategyCommand.Change command) =>
            this.Apply<StrategyEvent.Change>(command);
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="command"></param>
        public void ChangeStatus(StrategyCommand.ChangeStatus command) =>
            this.Apply<StrategyEvent.ChangeStatus>(command);
        /// <summary>
        /// 添加规则集合
        /// </summary>
        /// <param name="combines"></param>
        public void AddRule(IEnumerable<RuleCombine> combines)
        {
            RuleCombine combine = null;
            if (combines != null && combines.Any())
            {
                combine = combines.First();
                if (combines.Count() > 1)
                {
                    foreach (var c in combines.Skip(1))
                    {
                        combine.Add(c);
                    }
                }
            }

            this.Apply(new StrategyEvent.AddRule
            {
                RuleCombine = combine
            });
        }
    }
    /// <summary>
    /// 规则组合
    /// </summary>
    public class RuleCombine
    {
        /// <summary>
        /// 左值
        /// </summary>
        ///<remarks>
        /// <see cref="Rule.Id"/>标识
        /// </remarks>
        public string Left { get; set; }
        /// <summary>
        /// 右值
        /// </summary>
        public RuleCombine Right { get; set; }
        /// <summary>
        /// 逻辑操作符
        /// </summary>
        public LogicalOperator Operator { get; set; }
        /// <summary>
        /// 添加组合
        /// </summary>
        /// <param name="combine"></param>
        public void Add(RuleCombine combine)
        {
            if (this.Right == null)
            {
                this.Right = combine;
            }
            else
            {
                this.Right.Add(combine);
            }
        }
        /// <summary>
        /// 将表达式组合成表达式
        /// </summary>
        /// <param name="provider">rule存储委托</param>
        public async Task<string> CombineAsync(Func<string, Task<Rule>> provider)
        {
            var rule = await provider(this.Left);
            if (this.Right == null)
            {
                return rule.ToString();
            }
            else
            {
                var op = this.Operator == LogicalOperator.And ? "&&" : "||";
                return $"{rule} {op} {await this.Right.CombineAsync(provider)}";
            }
        }

        /// <summary>
        /// 获得组合内的所有规则标识
        /// </summary>
        public async Task<IEnumerable<string>> GetRuleIdsAsync()
        {
            var ids = new List<string>();

            ids.Add(this.Left);

            if (this.Right != null)
            {
                ids.AddRange(await this.Right.GetRuleIdsAsync());
            }

            return ids.Distinct().ToArray();
        }
    }
}