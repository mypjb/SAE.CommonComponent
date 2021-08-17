using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.ConfigServer.Handlers
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

        public async Task<string> HandleAsync(TemplateCommand.Create command)
        {
            Assert.Build(this._storage.AsQueryable<TemplateDto>()
                             .Any(s => s.Name.Equals(command.Name)))
                  .False($"Tempate '{command.Name}' exist");

            var template = await this.AddAsync(new Template(command));
            return template.Id;
        }

        public Task HandleAsync(TemplateCommand.Change command)
        {
            Assert.Build(this._storage.AsQueryable<TemplateDto>()
                             .Any(s => s.Name.Equals(command.Name) && s.Id != command.Id))
                  .False($"Tempate '{command.Name}' exist");

            return this.UpdateAsync(command.Id, s => s.Change(command));
        }

        public Task HandleAsync(Command.Delete<Template> command)
        {
            return this.DeleteAsync(command.Id);
        }

        public Task<TemplateDto> HandleAsync(Command.Find<TemplateDto> command)
        {
            return Task.FromResult(this._storage.AsQueryable<TemplateDto>()
                .FirstOrDefault(s => s.Id == command.Id));
        }

        public async Task<IPagedList<TemplateDto>> HandleAsync(TemplateCommand.Query command)
        {
            var query = this._storage.AsQueryable<TemplateDto>();

            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return PagedList.Build(query, command);
        }

        public async Task<IEnumerable<TemplateDto>> HandleAsync(Command.List<TemplateDto> command)
        {
            return this._storage.AsQueryable<TemplateDto>().ToList();
        }
    }
}