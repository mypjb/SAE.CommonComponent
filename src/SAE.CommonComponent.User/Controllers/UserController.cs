﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.User.Abstract.Dtos;
using SAE.CommonComponent.User.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.User.Controllers
{
    [Route("{controller}")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute]Command.Find<UserDto> command)
        {
            var dto= await this._mediator.SendAsync<UserDto>(command);
            return dto;
        }

        [AllowAnonymous]
        [HttpPost("{action}")]
        public async Task<object> Register(UserCommand.Register command)
        {
            var id = await this._mediator.SendAsync<string>(command);
            return id;
        }

        [HttpPut("{action}")]
        public Task Password(UserCommand.ChangePassword command)
        {
            return  this._mediator.SendAsync(command);
        }
        [HttpPut("{action}")]
        public Task Status(UserCommand.ChangeStatus command)
        {
            return this._mediator.SendAsync(command);
        }

        [HttpGet]
        [HttpPost]
        [Route("{action}")]
        public Task<IPagedList<UserDto>> Paging(UserCommand.Query command)
        {
            return this._mediator.SendAsync<IPagedList<UserDto>>(command);
        }
    }
}
