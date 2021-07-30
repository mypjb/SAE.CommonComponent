using Microsoft.AspNetCore.Hosting;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.ConfigServer.Test;
using SAE.CommonComponent.Test;
using SAE.CommonLibrary.Extension;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
using Microsoft.Extensions.DependencyInjection;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonLibrary.Abstract.Model;
using AppCommand = SAE.CommonComponent.Application.Commands.AppCommand;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.Application.Test
{
    public class AppControllerTest : BaseTest
    {
        public const string API = "app";
        private readonly SolutionControllerTest _solutionControllerTest;
        private readonly ProjectControllerTest _projectControllerTest;

        public AppControllerTest(ITestOutputHelper output) : base(output)
        {
            this._solutionControllerTest = new SolutionControllerTest(this._output);
            this._projectControllerTest = new ProjectControllerTest(this._output);
        }

        protected override IWebHostBuilder UseStartup(IWebHostBuilder builder)
        {
            return builder.UseStartup<Startup>()
                          .ConfigureServices(services =>
                          {
                              //services.AddSingleton(p =>
                              //{
                              //    return this._projectControllerTest.ServiceProvider.GetService<ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDto>>>();
                              //});

                              services.AddSingleton(p =>
                              {
                                  return this._projectControllerTest.ServiceProvider.GetService<ICommandHandler<ProjectCommand.Query, IPagedList<ProjectDetailDto>>>();
                              });

                              //services.AddSingleton(p =>
                              //{
                              //    return this._projectControllerTest.ServiceProvider.GetService<ICommandHandler<Command.Find<ProjectDto>, ProjectDto>>();
                              //});


                              services.AddSingleton(p =>
                              {
                                  return this._projectControllerTest.ServiceProvider.GetService<ICommandHandler<Command.Find<ProjectDetailDto>, ProjectDetailDto>>();
                              });


                              services.AddSingleton(p =>
                              {
                                  return this._solutionControllerTest.ServiceProvider.GetService<ICommandHandler<SolutionCommand.Query, IPagedList<SolutionDto>>>();
                              });
                          });
        }

        [Fact]
        public async Task<AppDto> Add()
        {
            var command = new AppCommand.Create
            {
                Name = this.GetRandom(),
                Scopes = new string[] { this.GetRandom(), this.GetRandom() },
                Endpoint = new Dtos.EndpointDto
                {
                    PostLogoutRedirectUris = new[] { this.GetRandom() },
                    RedirectUris = new[] { this.GetRandom() },
                    SignIn = this.GetRandom()
                }
            };
            var message = new HttpRequestMessage(HttpMethod.Post, API);
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            var id = await responseMessage.AsAsync<string>();
            var app = await this.Get(id);
            this.WriteLine(app);
            Assert.Equal(command.Name, app.Name);
            Assert.Contains(command.Scopes, app.Scopes.Contains);
            Assert.Equal(command.Endpoint.SignIn, app.Endpoint.SignIn);
            Assert.Equal(command.Endpoint.PostLogoutRedirectUris.First(), app.Endpoint.PostLogoutRedirectUris.First());
            Assert.Equal(command.Endpoint.RedirectUris.First(), app.Endpoint.RedirectUris.First());

            return app;
        }

        [Fact]
        public async Task Edit()
        {
            var app = await this.Add();
            var message = new HttpRequestMessage(HttpMethod.Put, API);
            var command = new AppCommand.Change
            {
                Id = app.Id,
                Name = this.GetRandom(),
                Scopes = new string[] { this.GetRandom(), this.GetRandom() },
                Endpoint = new Dtos.EndpointDto
                {
                    PostLogoutRedirectUris = new[] { this.GetRandom() },
                    RedirectUris = new[] { this.GetRandom() },
                    SignIn = this.GetRandom()
                }
            };
            message.AddJsonContent(command);
            var responseMessage = await this.HttpClient.SendAsync(message);
            responseMessage.EnsureSuccessStatusCode();
            var newApp = await this.Get(app.Id);
            Assert.Equal(command.Name, newApp.Name);
            Assert.Contains(command.Scopes, newApp.Scopes.Contains);
            Assert.Equal(command.Endpoint.SignIn, newApp.Endpoint.SignIn);
            Assert.Equal(command.Endpoint.PostLogoutRedirectUris.First(), newApp.Endpoint.PostLogoutRedirectUris.First());
            Assert.Equal(command.Endpoint.RedirectUris.First(), newApp.Endpoint.RedirectUris.First());
        }

        [Fact]
        public async Task Refresh()
        {
            var app = await this.Add();

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/{nameof(Refresh)}/{app.Id}");

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var secret = await responseMessage.Content.ReadAsStringAsync();

            var dto = await this.Get(app.Id);

            Assert.NotEqual(app.Secret, dto.Secret);
            this.WriteLine(secret);
        }

        [Fact]
        public async Task ChangeStatus()
        {
            var app = await this.Add();

            var command = new AppCommand.ChangeStatus
            {
                Id = app.Id,
                Status = app.Status == Status.Enable ? Status.Disable : Status.Enable
            };

            var message = new HttpRequestMessage(HttpMethod.Put, $"{API}/Status")
                                   .AddJsonContent(command);

            var responseMessage = await this.HttpClient.SendAsync(message);

            responseMessage.EnsureSuccessStatusCode();

            var dto = await this.Get(app.Id);

            Assert.Equal(command.Status, dto.Status);
        }

        //[Fact]
        //public async Task<AppDto> DistributionSolution()
        //{
        //    var app = await this.Add();

        //    var solution = await this._solutionControllerTest.Add();

        //    var command = new AppCommand.DistributionSolution
        //    {
        //        Id = app.Id,
        //        SolutionId = solution.Id
        //    };

        //    var message = new HttpRequestMessage(HttpMethod.Post, $"{API}/Solution")
        //                           .AddJsonContent(command);

        //    var responseMessage = await this.HttpClient.SendAsync(message);

        //    responseMessage.EnsureSuccessStatusCode();

        //    var dto = await this.Get(app.Id);

        //    Assert.Equal(command.SolutionId, dto.SolutionId);

        //    return app;
        //}

        [Fact]
        public async Task<AppDto> ReferenceProject()
        {
            var app = await this.Add();
            var solution = await this._solutionControllerTest.Add();

            var project = await this._projectControllerTest.Add(solution.Id);

            await this._projectControllerTest.Add(solution.Id);

            var command = new AppCommand.ReferenceProject
            {
                Id = app.Id,
                ProjectId = project.Id
            };

            var url = $"{API}/Project";

            var message = new HttpRequestMessage(HttpMethod.Post, url)
                              .AddJsonContent(command);

            var httpResponseMessage = await this.HttpClient.SendAsync(message);

            httpResponseMessage.EnsureSuccessStatusCode();

            var queryUrl = $"{url}/Paging";
            

            var referencedQueryReq = new HttpRequestMessage(HttpMethod.Get, $"{queryUrl}?id={app.Id}&referenced=true");

            var queryRep = await this.HttpClient.SendAsync(referencedQueryReq);

            var projects = await queryRep.AsAsync<PagedList<ProjectDetailDto>>();
            
            var dto = await this.Get(app.Id);

            Assert.Equal(dto.ProjectId, command.ProjectId);

            Assert.Contains(projects, s => s.Id == command.ProjectId);

            var unReferencedQueryReq = new HttpRequestMessage(HttpMethod.Get, $"{queryUrl}?id={app.Id}&referenced=false");

            queryRep = await this.HttpClient.SendAsync(unReferencedQueryReq);

            projects = await queryRep.AsAsync<PagedList<ProjectDetailDto>>();
            this.WriteLine(projects);
            Assert.True(projects.All(s => s.Id != command.ProjectId));
            return dto;
        }


        private async Task<AppDto> Get(string id)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{API}/{id}");
            var responseMessage = await this.HttpClient.SendAsync(message);
            var app = await responseMessage.AsAsync<AppDto>();
            this.WriteLine(app);
            return app;
        }
    }
}
