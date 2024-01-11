using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Authorization.ABAC;
using SAE.CommonLibrary.Abstract.Mediator.Behavior;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Authorize.Test
{
    public class StrategyControllerTest : BaseTest
    {
        public const string API = "Strategy";
        private readonly RuleControllerTest _ruleController;

        public StrategyControllerTest(ITestOutputHelper output) : base(output)
        {
            this._ruleController = new RuleControllerTest(output, this.HttpClient);
        }

        internal StrategyControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {
            this._ruleController = new RuleControllerTest(output, this.HttpClient);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>().ConfigureAppConfiguration(c =>
                    {
                        c.AddInMemoryCollection(new Dictionary<string, string>()
                        {
                            {$"{RetryPipelineBehaviorOptions.Option}:{nameof(RetryPipelineBehaviorOptions.Num)}","10"}
                        });
                    });
        }

        public async Task<StrategyDto> Add()
        {
            var command = new StrategyCommand.Create
            {
                Name = $"test_{this.GetRandom()}",
                Description = "add Strategy"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var id = await httpResponse.AsAsync<string>();
            var strategy = await this.Get(id);

            Assert.Equal(strategy.Name, command.Name);
            Assert.Equal(strategy.Description, command.Description);

            return strategy;
        }
        [Fact]
        public async Task Edit()
        {
            var dto = await this.Add();
            var command = new StrategyCommand.Change
            {
                Id = dto.Id,
                Name = $"{dto.Name}_edit",
                Description = "edit Strategy"
            };
            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");
            request.AddJsonContent(command);
            await this.HttpClient.SendAsync(request);
            var strategy = await this.Get(dto.Id);

            Assert.Equal(strategy.Name, command.Name);
            Assert.Equal(strategy.Description, command.Description);
        }

        [Fact]
        public async Task Delete()
        {
            var dto = await this.Add();

            var command = new Command.BatchDelete<Strategy>
            {
                Ids = new[] { dto.Id }
            };
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var exception = await Assert.ThrowsAnyAsync<SAEException>(() => this.Get(dto.Id));

            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);

        }

        [Fact]
        public async Task AddRule()
        {
            var range = new Random().Next(50, 100);
            var strategyDto = await this.Add();

            var ruleDtos = new List<RuleDto>();

            await Enumerable.Range(0, range)
                    .ForEachAsync(async s =>
                    {
                        var rule = await this._ruleController.Add();
                        ruleDtos.Add(rule);
                    });

            var command = new StrategyCommand.AddRule
            {
                Id = strategyDto.Id,
                Combines = ruleDtos.Select(s => new StrategyCommand.AddRule.Combine
                {
                    Id = s.Id,
                    Operator = Math.Abs(s.Id.GetHashCode()) % 2 == 0 ?
                                    LogicalOperator.Or : LogicalOperator.And
                }).ToArray()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(AddRule)}");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var strategy = await this.Get(strategyDto.Id);

            foreach (var rule in ruleDtos)
            {
                Assert.Contains(strategy.Expression, rule.Left);
            }

            this.WriteLine(strategy.Expression);
        }

        private async Task<StrategyDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<StrategyDto>();
        }

    }
}
