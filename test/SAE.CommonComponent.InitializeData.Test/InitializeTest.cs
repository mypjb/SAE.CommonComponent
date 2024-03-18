using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization.ABAC;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Plugin;
using SAE.CommonLibrary.AspNetCore.Plugin;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
namespace SAE.CommonComponent.InitializeData.Test
{
    public class InitializeTest : BaseTest
    {
        public InitializeTest(ITestOutputHelper output) : base(output)
        {

        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            var _ = typeof(SAE.CommonComponent.Application.Startup);
            _ = typeof(SAE.CommonComponent.Authorize.Startup);
            _ = typeof(SAE.CommonComponent.BasicData.Startup);
            _ = typeof(SAE.CommonComponent.ConfigServer.Startup);
            _ = typeof(SAE.CommonComponent.Identity.Startup);
            _ = typeof(SAE.CommonComponent.InitializeData.Startup);
            _ = typeof(SAE.CommonComponent.MultiTenant.Startup);
            _ = typeof(SAE.CommonComponent.OAuth.OAuthPlugin);
            _ = typeof(SAE.CommonComponent.PluginManagement.Startup);
            _ = typeof(SAE.CommonComponent.Routing.Startup);
            _ = typeof(SAE.CommonComponent.User.Startup);
            return builder.UseStartup<Master.Startup>()
                          .ConfigureServices(services =>
                          {
                              services.PostConfigureAll<JwtBearerOptions>(options =>
                              {
                                  if (this.HttpClient == null)
                                  {
                                      throw new Exception("http client null");
                                  }
                                  options.Backchannel.Use(new ProxyMessageHandler(() => this.HttpClient));
                              });
                          });
        }

        protected override void Configure(SAEOptions options)
        {
            options.Client = new HttpClient().Use(new ProxyMessageHandler(() => this.HttpClient, false));
            options.OAuth.Client = new HttpClient().Use(new ProxyMessageHandler(() => this.HttpClient));
            options.PollInterval = 5;
        }

        [Fact]
        public async Task InitialTest()
        {
            var pluginManage = this.ServiceProvider.GetService<IPluginManage>();
            this.WriteLine(new { pluginManage.Plugins });
            Assert.NotEmpty(pluginManage.Plugins);
            var rep = await this.HttpClient.GetAsync("/menu/tree");
            var json = await rep.Content.ReadAsStringAsync();
            this.WriteLine(json);
            rep.EnsureSuccessStatusCode();
        }

        private async Task SetOAuthAsync()
        {
            var mediator = this.ServiceProvider.GetService<IMediator>();

            var clusterDtos = await mediator.SendAsync<IEnumerable<AppClusterDto>>(new AppClusterCommand.List());

            var appDtos = await mediator.SendAsync<IEnumerable<AppDto>>(new AppCommand.List
            {
                ClusterId = clusterDtos.First().Id
            });

            var clients = await mediator.SendAsync<IPagedList<ClientDto>>(new ClientCommand.Query
            {
                AppId = appDtos.First().Id
            });

            var client = clients.First();

            var disco = await this.HttpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = this.HttpClient.BaseAddress.ToString(),
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            });

            Assert.False(disco.IsError, disco.Error);

            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = client.Id,
                ClientSecret = client.Secret,
                Scope = client.Scopes.Aggregate((a, b) => $"{a},{b}")
            };

            // request token
            var tokenResponse = await HttpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);

            Assert.False(tokenResponse.IsError, tokenResponse.Error);

            this.WriteLine(tokenResponse.Json);

            this.HttpClient.SetBearerToken(tokenResponse.AccessToken);
        }

        [Fact]
        public async Task AuthorizeTest()
        {
            await this.SetOAuthAsync();

            this.WriteLine(this.HttpClient.DefaultRequestHeaders.Authorization);

            Thread.Sleep(1 * 1000 * 60);
            var rep = await this.HttpClient.GetAsync("/cluster/list");

            Assert.True(rep.IsSuccessStatusCode);

            this.WriteLine(rep.StatusCode);

            var appClusterDtos = await rep.AsAsync<AppClusterDto[]>();

            this.WriteLine(appClusterDtos);
        }

        public class ProxyMessageHandler : DelegatingHandler
        {
            private readonly Func<HttpClient> _httpClientDelegate;
            private readonly bool _clearToken;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="delegate"></param>
            /// <param name="clearToken"></param>
            public ProxyMessageHandler(Func<HttpClient> @delegate, bool clearToken = true)
            {
                this._httpClientDelegate = @delegate;
                this._clearToken = clearToken;
            }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var client = _httpClientDelegate.Invoke() ?? new HttpClient();

                var auth = client.DefaultRequestHeaders.Authorization;

                if (this._clearToken && auth != null)
                {
                    client.DefaultRequestHeaders.Authorization = null;
                }

                var rep = await client.SendAsync(request.Clone(), cancellationToken);

                if (this._clearToken && auth != null)
                {
                    client.DefaultRequestHeaders.Authorization = auth;
                }

                return rep;
            }
        }
    }
}