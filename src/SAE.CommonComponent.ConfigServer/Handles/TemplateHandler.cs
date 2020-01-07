using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Models;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class TemplateHandler : AbstractHandler<Template>,
                                   IRequestHandler<TemplateCreateCommand,string>,
                                   ICommandHandler<TemplateChangeCommand>,
                                   ICommandHandler<RemoveCommand<Template>>,
                                   IRequestHandler<GetByIdCommand<Template>, TemplateDto>
    {
        public TemplateHandler(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<string> Handle(TemplateCreateCommand command)
        {
            var template= await this.Add(new Template(command));
            return template.Id;
        }

        public Task Handle(TemplateChangeCommand command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }

        public Task Handle(RemoveCommand<Template> command)
        {
            return this.Remove(command.Id);
        }

        public Task<TemplateDto> Handle(GetByIdCommand<Template> command)
        {
            throw new NotImplementedException();
        }
    }
}
