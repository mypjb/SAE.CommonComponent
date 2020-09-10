using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class TemplateHandler : AbstractHandler<Template>,
        ICommandHandler<TemplateCommand.Create, string>,
        ICommandHandler<TemplateCommand.Change>,
        ICommandHandler<Command.Delete<Template>>,
        ICommandHandler<Command.Find<TemplateDto>, TemplateDto>,
        ICommandHandler<TemplateCommand.Query, IPagedList<TemplateDto>>,
        ICommandHandler<Command.List<TemplateDto>, IEnumerable<TemplateDto>>
    {
        private readonly IStorage _storage;
        public TemplateHandler(IDocumentStore documentStore, IStorage storage) : base(documentStore)
        {
            this._storage = storage;
        }

        public async Task<string> Handle(TemplateCommand.Create command)
        {
            var template = await this.Add(new Template(command));
            return template.Id;
        }

        public Task Handle(TemplateCommand.Change command)
        {
            return this.Update(command.Id, s => s.Change(command));
        }

        public Task Handle(Command.Delete<Template> command)
        {
            return this.Remove(command.Id);
        }

        public Task<TemplateDto> Handle(Command.Find<TemplateDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<TemplateDto>()
                .FirstOrDefault(s => s.Id == command.Id));
        }

        public async Task<IPagedList<TemplateDto>> Handle(TemplateCommand.Query command)
        {
            return PagedList.Build(this._storage.AsQueryable<TemplateDto>(), command);
        }

        public async Task<IEnumerable<TemplateDto>> Handle(Command.List<TemplateDto> command)
        {
            return this._storage.AsQueryable<TemplateDto>().ToList();
        }
    }
}