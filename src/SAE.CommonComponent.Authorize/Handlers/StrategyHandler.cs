using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Authorize.Events;
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
    public class StrategyHandler : AbstractHandler<Strategy>,
                                   ICommandHandler<Command.BatchDelete<Strategy>>,
                                   ICommandHandler<Command.Find<StrategyDto>, StrategyDto>,
                                   ICommandHandler<StrategyCommand.Query, IPagedList<StrategyDto>>,
                                   ICommandHandler<StrategyCommand.List, IEnumerable<StrategyDto>>,
                                   ICommandHandler<StrategyCommand.Create, string>,
                                   ICommandHandler<StrategyCommand.Change>,
                                   ICommandHandler<StrategyCommand.ChangeStatus>,
                                   ICommandHandler<StrategyCommand.RuleList, IEnumerable<RuleDto>>,
                                   ICommandHandler<StrategyCommand.AddRule>

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
        }

        public async Task HandleAsync(StrategyCommand.ChangeStatus command)
        {
            await this.UpdateAsync(command.Id, strategy =>
            {
                strategy.ChangeStatus(command);
            });
        }

        public async Task HandleAsync(Command.BatchDelete<Strategy> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                await this._storage.DeleteAsync<Strategy>(id);
            });
        }

        public async Task<IPagedList<StrategyDto>> HandleAsync(StrategyCommand.Query command)
        {
            var query = this._storage.AsQueryable<StrategyDto>();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name) ||
                                    s.Description.Contains(command.Name));

            var dtos = PagedList.Build(query, command);

            await this._director.Build(dtos.AsEnumerable());

            return dtos;
        }

        public async Task<StrategyDto> HandleAsync(Command.Find<StrategyDto> command)
        {
            var dto = this._storage.AsQueryable<StrategyDto>()
                                    .FirstOrDefault(s => s.Id == command.Id);
            await this._director.Build(dto);
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

                    var findsCommand = new RuleCommand.Finds
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
        }

        private Task<Strategy> FindStrategy(Strategy Strategy)
        {
            var oldStrategy = this._storage.AsQueryable<Strategy>()
                                    .FirstOrDefault(s => s.Name == Strategy.Name
                                                    && s.Id != Strategy.Id);
            return Task.FromResult(oldStrategy);
        }
    }
}
