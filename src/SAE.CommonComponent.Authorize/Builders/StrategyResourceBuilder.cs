using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.Authorize.Builders
{
    public class StrategyResourceBuilder : IBuilder<IEnumerable<StrategyResourceDto>>
    {
        private readonly IMediator _mediator;

        public StrategyResourceBuilder(IMediator mediator)
        {
            this._mediator = mediator;
        }
        public async Task Build(IEnumerable<StrategyResourceDto> strategyResources)
        {
            var strategyIds = strategyResources.Select(s => s.StrategyId)
                                               .Distinct()
                                               .ToArray();

            var strategies = await this._mediator.SendAsync<IEnumerable<StrategyDto>>(new Command.Finds<StrategyDto>
            {
                Ids = strategyIds
            });

            if (!strategies.Any()) return;

            foreach (var strategyResource in strategyResources)
            {
                strategyResource.Strategy = strategies.FirstOrDefault(s => strategyResource.StrategyId == s.Id);
            }

        }
    }
}
