using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.Handlers
{
    /// <summary>
    /// 策略处理器
    /// </summary>
    public class StrategyHandler : AbstractHandler<Strategy>,
                                   ICommandHandler<Command.BatchDelete<Strategy>>,
                                   ICommandHandler<Command.Find<StrategyDto>, StrategyDto>,
                                   ICommandHandler<Command.Finds<StrategyDto>, IEnumerable<StrategyDto>>,
                                   ICommandHandler<StrategyCommand.Query, IPagedList<StrategyDto>>,
                                   ICommandHandler<StrategyCommand.List, IEnumerable<StrategyDto>>,
                                   ICommandHandler<StrategyCommand.Create, string>,
                                   ICommandHandler<StrategyCommand.Change>,
                                   ICommandHandler<StrategyCommand.ChangeStatus>,
                                   ICommandHandler<StrategyCommand.RuleList, IEnumerable<RuleDto>>,
                                   ICommandHandler<StrategyCommand.AddRule>,
                                   ICommandHandler<StrategyResourceCommand.Create, string>,
                                   ICommandHandler<StrategyResourceCommand.List, IEnumerable<StrategyDto>>,
                                   ICommandHandler<StrategyResourceCommand.List, IEnumerable<StrategyResourceDto>>,
                                   ICommandHandler<Command.BatchDelete<StrategyResource>>

    {
        private readonly IStorage _storage;
        private readonly IDirector _director;
        private readonly IMediator _mediator;
        private readonly IMessageQueue _messageQueue;
        private readonly ILogging _logging;

        public StrategyHandler(IDocumentStore documentStore,
                               IStorage storage,
                               IDirector director,
                               IMediator mediator,
                               IMessageQueue messageQueue,
                               ILogging<StrategyHandler> logging) : base(documentStore)
        {
            this._storage = storage;
            this._director = director;
            this._mediator = mediator;
            this._messageQueue = messageQueue;
            this._logging = logging;
        }

        public async Task<string> HandleAsync(StrategyCommand.Create command)
        {
            var strategy = new Strategy(command);
            await strategy.NameExist(this.FindStrategy);
            await this.AddAsync(strategy);
            // var strategyEvent = Strategy.To<StrategyEvent.Create>();
            // await this._messageQueue.PublishAsync(strategyEvent);
            return strategy.Id;
        }

        public async Task HandleAsync(StrategyCommand.Change command)
        {
            await this.UpdateAsync(command.Id, async strategy =>
            {
                strategy.Change(command);
                await strategy.NameExist(this.FindStrategy);
            });
            await this._messageQueue.PublishAsync(command);
        }

        public async Task HandleAsync(StrategyCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, strategy =>
            {
                strategy.ChangeStatus(command);
            });

            await this._messageQueue.PublishAsync(command);
        }

        public async Task HandleAsync(Command.BatchDelete<Strategy> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                await this._storage.DeleteAsync<Strategy>(id);
            });
            await this._messageQueue.PublishAsync(command);
        }

        public async Task<IPagedList<StrategyDto>> HandleAsync(StrategyCommand.Query command)
        {
            var query = this._storage.AsQueryable<StrategyDto>();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name) ||
                                    s.Description.Contains(command.Name));

            var dtos = PagedList.Build(query, command);

            await this._director.BuildAsync(dtos.AsEnumerable());

            return dtos;
        }

        public async Task<StrategyDto> HandleAsync(Command.Find<StrategyDto> command)
        {
            var dto = this._storage.AsQueryable<StrategyDto>()
                                    .FirstOrDefault(s => s.Id == command.Id);
            await this._director.BuildAsync(dto);
            return dto;
        }

        public async Task<IEnumerable<StrategyDto>> HandleAsync(StrategyCommand.List command)
        {
            var query = this._storage.AsQueryable<StrategyDto>();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name) ||
                                    s.Description.Contains(command.Name));

            if (command.Status != Status.ALL)
            {
                query = query.Where(s => s.Status == command.Status);
            }

            return query.ToArray();
        }

        public async Task<IEnumerable<RuleDto>> HandleAsync(StrategyCommand.RuleList command)
        {
            IEnumerable<RuleDto> dtos;
            if (command.Id.IsNullOrWhiteSpace())
            {
                dtos = Enumerable.Empty<RuleDto>();
            }
            else
            {
                var strategy = await this.FindAsync(command.Id);

                if (strategy != null && strategy.RuleCombine != null)
                {
                    var ids = await strategy.RuleCombine.GetRuleIdsAsync();

                    var findsCommand = new Command.Finds<RuleDto>
                    {
                        Ids = ids
                    };

                    dtos = await this._mediator.SendAsync<IEnumerable<RuleDto>>(findsCommand);
                }
                else
                {
                    dtos = Enumerable.Empty<RuleDto>();
                }

            }

            return dtos;
        }

        public async Task HandleAsync(StrategyCommand.AddRule command)
        {
            var strategy = await this.FindAsync(command.Id);

            if (strategy == null)
            {
                return;
            }

            IEnumerable<RuleCombine> combines;

            if (command.Combines != null && command.Combines.Length > 0)
            {
                combines = command.Combines.Select(s => new RuleCombine
                {
                    Left = s.Id,
                    Operator = s.Operator
                }).ToArray();
            }
            else
            {
                combines = null;
            }

            strategy.AddRule(combines);

            await strategy.BuildAsync(async ruleId =>
            {
                var ruleDto = await this._mediator.SendAsync<RuleDto>(new Command.Find<RuleDto>
                {
                    Id = ruleId
                });

                return ruleDto.To<Rule>();
            });

            await this._documentStore.SaveAsync(strategy);

            await this._messageQueue.PublishAsync(command);

        }

        public async Task<string> HandleAsync(StrategyResourceCommand.Create command)
        {
            var sr = new StrategyResource(command);
            await this._documentStore.SaveAsync(sr);
            return sr.Id;
        }

        public async Task<IEnumerable<StrategyDto>> HandleAsync(StrategyResourceCommand.List command)
        {
            var query = this._storage.AsQueryable<StrategyResource>().Where(s => s.ResourceType == command.ResourceType);

            if (!command.ResourceId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.ResourceId == command.ResourceId);
            }

            var ids = query.Select(s => s.StrategyId).ToArray().Distinct().ToArray();

            return await this._mediator.SendAsync<IEnumerable<StrategyDto>>(new Command.Finds<StrategyDto>
            {
                Ids = ids
            });

        }

        public async Task HandleAsync(Command.BatchDelete<StrategyResource> command)
        {
            await this._documentStore.DeleteAsync<StrategyResource>(command.Ids);
        }

        public async Task<IEnumerable<StrategyDto>> HandleAsync(Command.Finds<StrategyDto> command)
        {
            if (command.Ids == null || !command.Ids.Any()) return Enumerable.Empty<StrategyDto>();

            var result = this._storage.AsQueryable<StrategyDto>()
                                    .Where(s => command.Ids.Contains(s.Id))
                                    .ToArray();

            return result;
        }

        private Task<Strategy> FindStrategy(Strategy Strategy)
        {
            var oldStrategy = this._storage.AsQueryable<Strategy>()
                                    .FirstOrDefault(s => s.Name == Strategy.Name
                                                    && s.Id != Strategy.Id);
            return Task.FromResult(oldStrategy);
        }

        async Task<IEnumerable<StrategyResourceDto>> ICommandHandler<StrategyResourceCommand.List, IEnumerable<StrategyResourceDto>>.HandleAsync(StrategyResourceCommand.List command)
        {
            var query = this._storage.AsQueryable<StrategyResourceDto>().Where(s => s.ResourceType == command.ResourceType);

            if (!command.ResourceId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.ResourceId == command.ResourceId);
            }

            IEnumerable<StrategyResourceDto> result = query.ToArray();

            await this._director.BuildAsync(result);

            return result;
        }
    }
}
