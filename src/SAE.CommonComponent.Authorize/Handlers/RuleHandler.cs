using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Authorize.Handlers
{
    /// <summary>
    /// 规则处理器
    /// </summary>
    public class RuleHandler : AbstractHandler<Rule>,
                               ICommandHandler<RuleCommand.Create, string>,
                               ICommandHandler<RuleCommand.Change>,
                               ICommandHandler<Command.BatchDelete<Rule>>,
                               ICommandHandler<Command.Find<RuleDto>, RuleDto>,
                               ICommandHandler<Command.Finds<RuleDto>, IEnumerable<RuleDto>>,
                               ICommandHandler<RuleCommand.Query, IPagedList<RuleDto>>,
                               ICommandHandler<RuleCommand.List, IEnumerable<RuleDto>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IMessageQueue _messageQueue;

        public RuleHandler(IDocumentStore documentStore,
                           IStorage storage,
                           IMediator mediator,
                           IMessageQueue messageQueue) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._messageQueue = messageQueue;
        }

        public async Task<string> HandleAsync(RuleCommand.Create command)
        {
            var rule = new Rule(command);
            await rule.NameExist(this.FindRule);
            await this.AddAsync(rule);
            return rule.Id;
        }

        public async Task HandleAsync(RuleCommand.Change command)
        {
            await this.UpdateAsync(command.Id, async rule =>
            {
                rule.Change(command);
                await rule.NameExist(this.FindRule);
            });
            await this._messageQueue.PublishAsync(command);
        }

        public async Task HandleAsync(Command.BatchDelete<Rule> command)
        {
            await command.Ids.ForEachAsync(async id =>
            {
                await this._storage.DeleteAsync<Rule>(id);
            });
            await this._messageQueue.PublishAsync(command);
        }

        public Task<IPagedList<RuleDto>> HandleAsync(RuleCommand.Query command)
        {
            var query = this.GetStorage();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name) ||
                                    s.Description.Contains(command.Name));

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task<IEnumerable<RuleDto>> HandleAsync(RuleCommand.List command)
        {
            var query = this.GetStorage();

            if (!command.Name.IsNullOrWhiteSpace())
                query = query.Where(s => s.Name.Contains(command.Name) ||
                                    s.Description.Contains(command.Name));

            return query.ToArray();
        }

        public async Task<RuleDto> HandleAsync(Command.Find<RuleDto> command)
        {
            var dto = this.GetStorage()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        private IQueryable<RuleDto> GetStorage()
        {
            return this._storage.AsQueryable<RuleDto>();
        }
        private Task<Rule> FindRule(Rule Rule)
        {
            var oldRule = this._storage.AsQueryable<Rule>()
                              .FirstOrDefault(s => s.Name == Rule.Name
                                              && s.Id != Rule.Id);
            return Task.FromResult(oldRule);
        }

        public Task<IEnumerable<RuleDto>> HandleAsync(Command.Finds<RuleDto> command)
        {
            return Task.FromResult<IEnumerable<RuleDto>>(
                        this.GetStorage()
                            .Where(s => command.Ids.Contains(s.Id))
                            .ToArray());
        }
    }
}
