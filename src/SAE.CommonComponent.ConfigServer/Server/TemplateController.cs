﻿using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class TemplateController : Controller
    {
        private readonly IMediator _mediator;

        public TemplateController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpPost]
        public async Task<object> Add(TemplateCreateCommand command)
        {
            return await this._mediator.Send<string>(command);
        }
        [HttpDelete("{id}")]
        public async Task<object> Delete(RemoveCommand<Template> command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpPut]
        public async Task<object> Put(TemplateChangeCommand command)
        {
            await this._mediator.Send(command);
            return ResponseResult.Success;
        }
        [HttpGet("{id}")]
        public async Task<object> Get(GetByIdCommand<Template> command)
        {
            return await this._mediator.Send<TemplateDto>(command);
        }
    }
}
