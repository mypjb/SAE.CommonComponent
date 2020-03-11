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
        public async Task<object> Add(ProjectCreateCommand command)
        {
            return await this._mediator.Send<string>(command);
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute]RemoveCommand<Project> command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpPut]
        public async Task<object> Put(ProjectChangeCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpGet("{id}")]
        public async Task<object> Get(string id)
        {
            return await this._mediator.Send<ProjectDto>(id);
        }

        [HttpPost("config/{action}")]
        public async Task<object> Relevance(ProjectRelevanceConfigCommand command)
        {
            await this._mediator.Send(command);

            return ResponseResult.Success;
        }
        
        [HttpGet("{action}/{id}")]
        public async Task<object> Config(string id)
        {
            return await this._mediator.Send<ProjectConfigDto>(id);
        }

        

        [HttpPut("config")]
        public async Task<object> ProjectConfig(ProjectConfigChangeAliasCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpDelete("config")]
        public async Task<object> ProjectConfig(BatchRemoveCommand<ProjectConfig> command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpGet("{action}")]
        public async Task<object> Paging([FromQuery]ProjectQueryCommand command)
        {
            return await this._mediator.Send<IPagedList<ProjectDto>>(command);
        }
        [HttpGet("config/paging")]
        public async Task<object> ProjectConfigPaging([FromQuery]ProjectConfigQueryCommand command)
        {
            return await this._mediator.Send<IPagedList<ProjectConfigDto>>(command);
        }

        [HttpGet("config/relevance")]
        public async Task<object> Relevance([FromQuery]ProjectConfigQueryCommand command)
        {
            return await this._mediator.Send<IPagedList<ConfigDto>>(command);
        }
    }
}
