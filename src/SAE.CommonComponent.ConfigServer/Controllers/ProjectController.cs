using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<object> Delete(RemoveCommand<Project> command)
        {
            await this._mediator.Send<string>(command);
            return ResponseResult.Success;
        }
        [HttpPut]
        public async Task<object> Put(ProjectChangeCommand command)
        {
            await this._mediator.Send<string>(command);
            return ResponseResult.Success;
        }
        [HttpGet("{id}")]
        public async Task<object> Get(GetByIdCommand<Config> command)
        {
            return await this._mediator.Send<ConfigDto>(command);
        }

        [HttpPost("{action}")]
        public async Task<object> Relevance(ProjectRelevanceConfigCommand command)
        {
            await this._mediator.Send<string>(command);

            return ResponseResult.Success;
        }
        [HttpPost("{action}/{id}")]
        public async Task<object> Config(GetByIdCommand<Config> command)
        {
            return await this._mediator.Send<ProjectConfigDto>(command);
        }

        [HttpPut("/project/config")]
        public async Task<object> ProjectConfig(ProjectConfigChangeAliasCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }

        [HttpDelete("/project/config")]
        public async Task<object> ProjectConfig(BatchRemoveCommand<ProjectConfig> command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
    }
}
