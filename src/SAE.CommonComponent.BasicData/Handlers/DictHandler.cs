using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Domains;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.BasicData.Handlers
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
        private readonly IDirector _director;

        public ConfigHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator,
            IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
        }

        private async Task PermutationAsync(DictItemDto dict, IEnumerable<DictItemDto> DictItems)
        {
            var items = DictItems.Where(s => s.ParentId == dict.Id).ToArray();
            dict.Items = items;
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

        private Task<bool> DictIsExistAsync(Dict dict)
        {
            return Task.FromResult(this._storage.AsQueryable<DictDto>()
                         .Count(s => s.ParentId == dict.ParentId &&
                                     s.Type == dict.Type &&
                                     s.Id != dict.Id &&
                                     s.Name == dict.Name) > 0);
        }

        public async Task<string> HandleAsync(DictCommand.Create command)
        {
            var dict = new Dict(command);
            await dict.ParentExist(this._documentStore.FindAsync<Dict>);
            await dict.NotExist(this.DictIsExistAsync);
            await this.AddAsync(dict);
            return dict.Id;
        }

        public async Task HandleAsync(DictCommand.Change command)
        {
            var dict = await this._documentStore.FindAsync<Dict>(command.Id);
            await dict.Change(command, this._documentStore.FindAsync<Dict>, this.DictIsExistAsync);
            await this._documentStore.SaveAsync(dict);
        }

        public async Task<DictDto> HandleAsync(Command.Find<DictDto> command)
        {
            var first = this._storage.AsQueryable<DictDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            if (first != null)
            {
                await this._director.Build<IEnumerable<DictDto>>(new[] { first });
            }
            
            return first;
        }

        public async Task<IPagedList<DictDto>> HandleAsync(DictCommand.Query command)
        {
            var query = this._storage.AsQueryable<DictDto>();
            if (command.Root)
            {
                query = query.Where(s => s.ParentId == Constants.Dict.RootId);
            }
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            if (command.Type > 0)
            {
                query = query.Where(s => s.Type == command.Type);
            }
            var paging = PagedList.Build(query, command);

            await this._director.Build(paging.AsEnumerable());
            return paging;
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

            var query = this._storage.AsQueryable<DictDto>();

            if (command.Id.IsNullOrWhiteSpace() || Constants.Dict.RootId.Equals(command.Id))
            {
                command.Id = Constants.Dict.RootId;
            }
            else
            {
                var dict = this._storage.AsQueryable<DictDto>().FirstOrDefault(s => s.Id == command.Id);
                command.Type = dict.Type;
                query = query.Where(s => s.Type == command.Type);
            }

            var dicts = query.Select(s => new DictItemDto
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
