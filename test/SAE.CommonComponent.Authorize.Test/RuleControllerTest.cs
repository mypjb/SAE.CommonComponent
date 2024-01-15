using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Domains;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.Authorize.Test
{

    public class RuleControllerTest : BaseTest
    {
        public const string API = "rule";
        public RuleControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        internal RuleControllerTest(ITestOutputHelper output, HttpClient httpClient) : base(output, httpClient)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>();
        }

        public async Task<RuleDto> Add(string[] args = null)
        {
            args = this.BuildArgs(args);

            var command = new RuleCommand.Create
            {
                Name = $"test_{this.GetRandom()}",
                Description = "add rule",
                Left = args[0],
                Symbol = args[1],
                Right = args[2]
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{API}");
            request.AddJsonContent(command);
            var httpResponse = await this.HttpClient.SendAsync(request);
            var id = await httpResponse.AsAsync<string>();
            var rule = await this.Get(id);

            Assert.Equal(rule.Name, command.Name);
            Assert.Equal(rule.Description, command.Description);
            Assert.Equal(rule.Left, command.Left);
            Assert.Equal(rule.Symbol, command.Symbol);
            Assert.Equal(rule.Right, command.Right);

            return rule;
        }

        [Fact]
        public async Task Edit()
        {
            var dto = await this.Add();

            var args = this.BuildArgs();

            var command = new RuleCommand.Change
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = "edit rule",
                Left = args[0],
                Symbol = args[1],
                Right = args[2]
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"{API}");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            var rule = await this.Get(dto.Id);

            Assert.Equal(rule.Name, command.Name);
            Assert.Equal(rule.Description, command.Description);
            Assert.Equal(rule.Left, command.Left);
            Assert.Equal(rule.Symbol, command.Symbol);
            Assert.Equal(rule.Right, command.Right);
        }

        [Fact]
        public async Task Delete()
        {
            var dto = await this.Add();

            var command = new Command.BatchDelete<Rule>
            {
                Ids = new[] { dto.Id }
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{API}");

            request.AddJsonContent(command);

            var httpResponse = await this.HttpClient.SendAsync(request);

            var exception = await Assert.ThrowsAnyAsync<SAEException>(() => this.Get(dto.Id));

            Assert.Equal((int)StatusCodes.ResourcesNotExist, exception.Code);

        }

        private string[] BuildArgs(string[] args = null)
        {
            if (args == null || args.Length == 0)
            {
                var symbols = new[] { ">", ">=", "<", "<=", "==", "!", "!=", "regex", "in" };

                var symbol = symbols[Math.Abs(this.GetRandom().GetHashCode()) % symbols.Length];

                args = new string[3]
                {
                    $"${this.GetRandom()}",
                    symbol,
                    symbol=="!"?string.Empty:$"'{this.GetRandom()}'",
                };
            }

            return args;
        }

        private async Task<RuleDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            return await responseMessage.AsAsync<RuleDto>();
        }
    }
}
