using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.ConfigServer.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class AppDataController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBitmapEndpointStorage _bitmapEndpointStorage;

        public AppDataController(IMediator mediator, IBitmapEndpointStorage bitmapEndpointStorage)
        {
            this._mediator = mediator;
            this._bitmapEndpointStorage = bitmapEndpointStorage;
        }

        private async Task<IActionResult> FindAsync(AppDataCommand.Find command)
        {
            
            var appConfig = await this._mediator.SendAsync<AppDataDto>(command);

            if (command.Version == appConfig.Version)
            {
                return this.StatusCode((int)HttpStatusCode.NotModified);
            }
            else
            {
                var query = new QueryString();
                foreach (var kv in this.Request.Query)
                {
                    if (!kv.Key.Equals(nameof(command.Version), StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Add(kv.Key, kv.Value);
                    }
                }

                query = query.Add(nameof(command.Version).ToLower(), appConfig.Version.ToString());


                var nextUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.Path}{query.ToUriComponent()}";

                this.HttpContext.Response.Headers.Add(SAEConfigurationProvider.ConfigUrl, nextUrl);

                return this.Json(appConfig.Data);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AppDataCommand.Find command)
        {
            var claim = this.User.FindFirst(Constants.Claim.AppId);

            command.AppId = claim.Value;

            command.Private = true;

            Assert.Build(command.AppId)
                  .NotNullOrWhiteSpace($"parameter appid invalid");

            return await this.FindAsync(command);
        }

        [AllowAnonymous]
        [HttpGet("{action}")]
        public async Task<IActionResult> Public([FromQuery] AppDataCommand.Find command)
        {
            var index = this._bitmapEndpointStorage.GetIndex(this.HttpContext);
            command.Private = false;

            Assert.Build(command.AppId)
                  .NotNullOrWhiteSpace($"parameter appid invalid");

            return await this.FindAsync(command);
        }
    }
}
