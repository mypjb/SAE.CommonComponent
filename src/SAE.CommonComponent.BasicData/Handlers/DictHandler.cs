using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Domains;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handlers
{
    public class ConfigHandler : AbstractHandler<Dict>,
                                 ICommandHandler<DictCommand.Create, string>,
                                 ICommandHandler<DictCommand.Change>,
                                 ICommandHandler<Command.Delete<Dict>>,
                                 ICommandHandler<Command.Find<DictDto>, DictDto>,
                                 ICommandHandler<DictCommand.Query, IPagedList<DictDto>>,
                                 ICommandHandler<DictCommand.Tree, IEnumerable<DictItemDto>>,
                                 ICommandHandler<DictCommand.List, IEnumerable<DictDto>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;

        public ConfigHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
        }

        private async Task PermutationAsync(DictItemDto Dict, IEnumerable<DictItemDto> DictItems)
        {
            var items = DictItems.Where(s => s.ParentId == Dict.Id).ToArray();
            Dict.Items = items;
            await items.ForEachAsync(async item => await this.PermutationAsync(item, DictItems));
        }

        private async Task ForEachTreeAsync(IEnumerable<DictItemDto> items, Func<DictItemDto, Task> func)
        {
            foreach (var item in items)
            {
                if (item.Items != null && item.Items.Any())
                    await this.ForEachTreeAsync(item.Items, func);

                await func(item);
            }
        }

        private Task<bool> DictIsExistAsync(Dict Dict)
        {
            return Task.FromResult(this._storage.AsQueryable<DictItemDto>()
                         .Count(s => s.ParentId == Dict.ParentId &&
                                     s.Type == Dict.Type &&
                                     s.Id != Dict.Id &&
                                     s.Name == Dict.Name) > 0);
        }

        public async Task<string> HandleAsync(DictCommand.Create command)
        {
            var Dict = new Dict(command);
            await Dict.ParentExist(this._documentStore.FindAsync<Dict>);
            await Dict.NotExist(this.DictIsExistAsync);
            await this.AddAsync(Dict);
            return Dict.Id;
        }

        public async Task HandleAsync(DictCommand.Change command)
        {
            var Dict = await this._documentStore.FindAsync<Dict>(command.Id);
            await Dict.Change(command, this._documentStore.FindAsync<Dict>, this.DictIsExistAsync);
            await this._documentStore.SaveAsync(Dict);
        }

        public async Task<DictDto> HandleAsync(Command.Find<DictDto> command)
        {
            var first = this._storage.AsQueryable<DictDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            return first;
        }

        public async Task<IPagedList<DictDto>> HandleAsync(DictCommand.Query command)
        {
            var query = this._storage.AsQueryable<DictDto>();
            if (command.Root)
            {
                query = query.Where(s => s.ParentId == Dict.DefaultId);
            }
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            if (command.Type > 0)
            {
                query = query.Where(s => s.Type == command.Type);
            }
            return PagedList.Build(query, command);
        }

        public async Task HandleAsync(Command.Delete<Dict> command)
        {
            var dict = await this._documentStore.FindAsync<Dict>(command.Id);

            Assert.Build(dict)
                  .NotNull("dict not exist");

            var roots = await this._mediator.SendAsync<IEnumerable<DictItemDto>>(new DictCommand.Tree
            {
                Id = dict.Id,
                Type = dict.Type
            });

            await this.ForEachTreeAsync(roots, async d =>
            {
                await this._documentStore.DeleteAsync<Dict>(d.Id);

            });

            await this._documentStore.DeleteAsync(dict);
        }

        public async Task<IEnumerable<DictItemDto>> HandleAsync(DictCommand.Tree command)
        {
            if (command.Type == 0)
            {
                if (command.Id.IsNullOrWhiteSpace())
                {
                    return Enumerable.Empty<DictItemDto>();
                }

                var dict = this._storage.AsQueryable<DictDto>().FirstOrDefault(s => s.Id == command.Id);
                command.Type = dict.Type;
            }
            else if (command.Id.IsNullOrWhiteSpace())
            {
                command.Id = Dict.DefaultId;
            }

            var dicts = this._storage.AsQueryable<DictDto>()
                                     .Select(s => new DictItemDto
                                     {
                                         Id = s.Id,
                                         Name = s.Name,
                                         ParentId = s.ParentId,
                                         Type = s.Type
                                     }).ToArray();

            var rootDicts = dicts.Where(s => s.ParentId == command.Id).ToArray();

            await rootDicts.ForEachAsync(async dict => await this.PermutationAsync(dict, dicts));

            return rootDicts;
        }

        public async Task<IEnumerable<DictDto>> HandleAsync(DictCommand.List command)
        {
            if (!command.Id.IsNullOrWhiteSpace())
            {
                return this._storage.AsQueryable<DictDto>()
                                    .Where(s => s.ParentId == command.Id)
                                    .ToList();
            }
            else if (command.Type > 0)
            {
                return this._storage.AsQueryable<DictDto>()
                                    .Where(s => s.Type == command.Type)
                                    .ToList();
            }
            else
            {
                return Enumerable.Empty<DictDto>();
            }
        }
    }
}
