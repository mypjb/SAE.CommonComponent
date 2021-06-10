﻿using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class ProjectController : Controller
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<object> Add(ProjectCommand.Create command)
        {
            return await this._mediator.SendAsync<string>(command);
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] Command.Delete<Project> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpPut]
        public async Task<object> Put(ProjectCommand.Change command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Command.Find<ProjectDto> command)
        {
            return await this._mediator.SendAsync<ProjectDto>(command);
        }

        [HttpPost("config/{action}")]
        public async Task<object> Relevance(ProjectCommand.RelevanceConfig command)
        {
            await this._mediator.SendAsync(command);

            return this.Ok();
        }

        [HttpGet("{action}/{id}")]
        public async Task<object> Config([FromRoute] Command.Find<ProjectConfigDto> command)
        {
            return await this._mediator.SendAsync<ProjectConfigDto>(command);
        }



        [HttpPut("config")]
        public async Task<object> ProjectConfig(ProjectCommand.ConfigChangeAlias command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpDelete("config")]
        public async Task<object> ProjectConfig(Command.BatchDelete<ProjectConfig> command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }

        [HttpPost("{action}")]
        public async Task<object> Publish(ProjectCommand.Publish command)
        {
            await this._mediator.SendAsync(command);
            return this.Ok();
        }
        [HttpGet("{action}")]
        public async Task<object> Preview([FromQuery]ProjectCommand.Preview command)
        {
            var data = await this._mediator.SendAsync<object>(command);
            return data;
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery] ProjectCommand.Query command)
        {
            return await this._mediator.SendAsync<IPagedList<ProjectDto>>(command);
        }
        [HttpGet("config/paging")]
        public async Task<object> ProjectConfigPaging([FromQuery] ProjectCommand.ConfigQuery command)
        {
            return await this._mediator.SendAsync<IPagedList<ProjectConfigDto>>(command);
        }

        [HttpGet("config/relevance/{action}")]
        public async Task<object> Paging([FromQuery] ProjectCommand.ConfigQuery command)
        {
            return await this._mediator.SendAsync<IPagedList<ConfigDto>>(command);
        }
    }
}
