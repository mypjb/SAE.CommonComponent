using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class TemplateHandler : AbstractHandler<Template>,
                                   ICommandHandler<TemplateCreateCommand, string>,
                                   ICommandHandler<TemplateChangeCommand>,
                                   ICommandHandler<RemoveCommand<Template>>,
                                   ICommandHandler<string, TemplateDto>,
                                   ICommandHandler<TemplateQueryCommand, IPagedList<TemplateDto>>
    {
        public TemplateHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore, storage)
        {
        }

        public async Task<string> Handle(TemplateCreateCommand command)
        {
            var template = await this.Add(new Template(command));
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

        public Task<TemplateDto> Handle(string command)
        {
            return Task.FromResult(this._storage.AsQueryable<TemplateDto>()
                                       .FirstOrDefault(s => s.Id == command));
        }

        public async Task<IPagedList<TemplateDto>> Handle(TemplateQueryCommand command)
        {
            return PagedList.Build(this._storage.AsQueryable<TemplateDto>(), command);
        }
    }
}
