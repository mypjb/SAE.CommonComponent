using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.User.Commands;
using SAE.CommonLibrary.Abstract.Mediator;
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

        [HttpPost("{action}")]
        public async Task<object> Register(UserCommand.Register register)
        {
            var id = await this._mediator.Send<string>(register);
            return id;
        }
    }
}
