using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Configuration;
using SAE.CommonLibrary.Configuration.Implement;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer
{
    internal class ConfigOptions
    {
        public string Id { get; set; }
    }
    internal class ConfigServerProvider : IOptionsProvider
    {
        private readonly IMediator _mediator;
        private readonly ConfigOptions _options;

        public ConfigServerProvider(IMediator mediator,ConfigOptions options)
        {
            this._mediator = mediator;
            this._options = options;
        }
        public event Func<Task> OnChange;

        public async Task HandleAsync(OptionsContext context)
        {
           var appConfig=  await this._mediator.Send<AppConfigDto>(new Commands.AppCommand.Config
            {
                 Id=this._options.Id
            });

            object data;
            if (appConfig.Data.TryGetValue(context.Name, out data))
            {
                context.SetOption(data.ToString().ToObject(context.Type));
                context.Provider = this;
            };
            
        }

        public Task SaveAsync(string name, object options)
        {
            throw new NotImplementedException();
        }
    }
}
