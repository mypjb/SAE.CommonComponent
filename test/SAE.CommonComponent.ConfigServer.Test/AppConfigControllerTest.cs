using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Application.Test;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonComponent.BasicData.Test;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SAE.CommonComponent.ConfigServer.Test
{
    public class AppConfigControllerTest : ControllerTest
    {
        public const string API = "/app/config";
        private DictControllerTest _dictControllerTest;
        private ConfigControllerTest _configController;
        private readonly AppControllerTest _appControllerTest;
        private readonly string _environmentId;
        public AppConfigControllerTest(ITestOutputHelper output) : base(output)
        {
            this._dictControllerTest = new DictControllerTest(this._output);
            var dictDto = this._dictControllerTest.Add(null).GetAwaiter().GetResult();
            this._environmentId = dictDto.Id;
            this._configController = new ConfigControllerTest(this._output, this.HttpClient, dictDto.Id);
            this._appControllerTest = new AppControllerTest(this._output);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return base.UseStartup(builder).ConfigureServices(services =>
            {
                services.AddSingleton(p =>
                {
                    return this._dictControllerTest.ServiceProvider.GetServices<ICommandHandler<DictCommand.List, IEnumerable<DictDto>>>();
                });

                services.AddSingleton(p =>
                {
                    return this._appControllerTest.ServiceProvider.GetServices<ICommandHandler<Command.Find<AppDto>, AppDto>>();
                });

                services.AddSingleton(p =>
                {
                    return this._appControllerTest.ServiceProvider.GetServices<ICommandHandler<Command.Find<AppDto>, AppDto>>();
                });
            });
        }


        [Fact]
        public async Task Reference()
        {
            var appDto = await _appControllerTest.Add();

            var configs = new List<ConfigDto>();
            await Enumerable.Range(0, new Random().Next(3, 10))
                      .ForEachAsync(async s =>
                      {
                          configs.Add(await this._configController.Add());
                      });

            var command = new AppConfigCommand.ReferenceConfig
            {
                AppId = appDto.Id,
                ConfigIds = configs.Select(s => s.Id).ToArray()
            };


            var message = new HttpRequestMessage(HttpMethod.Post, API)
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            httpResponseMessage.EnsureSuccessStatusCode();

            var responseMessage = await this.HttpClient.GetAsync($"{API}/paging?{nameof(command.AppId)}={command.AppId}&&{nameof(ConfigDto.EnvironmentId)}={this._environmentId}");

            var appConfigDtos = await responseMessage.AsAsync<PagedList<AppConfigDto>>();


            foreach (var appConfig in appConfigDtos)
            {
                var configDto = configs.First(s => s.Id == appConfig.ConfigId);
                Assert.Equal(configDto.Id, appConfig.Config.Id);
                Assert.Equal(configDto.ClusterId, appConfig.Config.ClusterId);
                Assert.Equal(configDto.EnvironmentId, appConfig.Config.EnvironmentId);
                Assert.Equal(configDto.Content, appConfig.Config.Content);
                Assert.Equal(configDto.Version, appConfig.Config.Version);
            }
        }

        [Fact]
        public async Task AppConfig()
        {
            var config = await this._configController.Add();
            var publicConfig = await this._configController.Add(config.ClusterId);
            var appDto = await _appControllerTest.Add(config.ClusterId);
            var command = new AppConfigCommand.ReferenceConfig
            {
                AppId = appDto.Id,
                ConfigIds = new[] { config.Id, publicConfig.Id }
            };

            var message = new HttpRequestMessage(HttpMethod.Post, API)
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            var appConfigsRep = await this.HttpClient.GetAsync($"{API}/paging?AppId={command.AppId}&EnvironmentId={config.EnvironmentId}");

            var appConfigs = await appConfigsRep.AsAsync<PagedList<AppConfigDto>>();

            var publicAppConfig = appConfigs.First(s => s.ConfigId == publicConfig.Id);

            var configChangeCommand = new AppConfigCommand.Change
            {
                Id = publicAppConfig.Id,
                Alias = this.GetRandom(),
                Private = false
            };

            var configChangeReq = new HttpRequestMessage(HttpMethod.Put, API);

            configChangeReq.AddJsonContent(configChangeCommand);

            (await this.HttpClient.SendAsync(configChangeReq)).EnsureSuccessStatusCode();

            var publishCommand = new AppConfigCommand.Publish
            {
                Id = appDto.Id,
                EnvironmentId = config.EnvironmentId
            };

            var publishReq = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(AppConfigCommand.Publish)}")
                              .AddJsonContent(publishCommand);

            (await this.HttpClient.SendAsync(publishReq)).EnsureSuccessStatusCode();

            var publishReq2 = new HttpRequestMessage(HttpMethod.Post, $"{API}/{nameof(AppConfigCommand.Publish)}")
                             .AddJsonContent(publishCommand);

            (await this.HttpClient.SendAsync(publishReq2)).EnsureSuccessStatusCode();

            var previewCommand = new AppConfigCommand.Preview
            {
                Id = appDto.Id,
                EnvironmentId = this._environmentId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{API}/Preview?{nameof(previewCommand.Id)}={previewCommand.Id}&{nameof(previewCommand.EnvironmentId)}={previewCommand.EnvironmentId}");

            var responseMessage = await this.HttpClient.SendAsync(requestMessage);

            var projectPreview = await responseMessage.AsAsync<AppConfigDataPreviewDto>();
            this.WriteLine(projectPreview);
            Assert.Contains(config.Content, projectPreview.Current.ToJsonString());
            Assert.Contains(publicConfig.Content, projectPreview.Current.ToJsonString());
        }

    }
}
