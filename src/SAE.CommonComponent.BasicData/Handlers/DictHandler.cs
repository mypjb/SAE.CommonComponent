using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Domains;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.BasicData.Handlers
{
    public class DictHandler : AbstractHandler<Dict>,
                                 ICommandHandler<DictCommand.Create, string>,
                                 ICommandHandler<DictCommand.Change>,
                                 ICommandHandler<Command.Delete<Dict>>,
                                 ICommandHandler<Command.Find<DictDto>, DictDto>,
                                 ICommandHandler<DictCommand.Query, IPagedList<DictDto>>,
                                 ICommandHandler<DictCommand.Tree, IEnumerable<DictItemDto>>,
                                 ICommandHandler<DictCommand.List, IEnumerable<DictDto>>,
                                 ICommandHandler<DictCommand.Find, DictDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;

        public DictHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator,
            IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
        }

        private async Task PermutationAsync(DictItemDto dict, IEnumerable<DictItemDto> dictItems)
        {
            var items = dictItems.Where(s => s.ParentId == dict.Id).ToArray();
            dict.Items = items;
            await items.ForEachAsync(async item => await this.PermutationAsync(item, dictItems));
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
                                     s.Id != dict.Id &&
                                     s.Name == dict.Name) > 0);
        }

        public async Task<string> HandleAsync(DictCommand.Create command)
        {
            var dict = new Dict(command);
            await dict.ParentExist(this.FindAsync);
            await dict.NotExist(this.DictIsExistAsync);
            await this.AddAsync(dict);
            return dict.Id;
        }

        public async Task HandleAsync(DictCommand.Change command)
        {
            var dict = await this.FindAsync(command.Id);
            await dict.Change(command, this.FindAsync, this.DictIsExistAsync);
            await this._documentStore.SaveAsync(dict);
        }

        public async Task<DictDto> HandleAsync(Command.Find<DictDto> command)
        {
            var first = this._storage.AsQueryable<DictDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            if (first != null)
            {
                await this._director.BuildAsync<IEnumerable<DictDto>>(new[] { first });
            }

            return first;
        }

        public async Task<IPagedList<DictDto>> HandleAsync(DictCommand.Query command)
        {
            var query = this._storage.AsQueryable<DictDto>();

            if (!string.IsNullOrWhiteSpace(command.ParentId))
            {
                query = query.Where(s => s.ParentId == command.ParentId);
            }

            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            var paging = PagedList.Build(query, command);

            await this._director.BuildAsync(paging.AsEnumerable());

            return paging;
        }

        public async Task HandleAsync(Command.Delete<Dict> command)
        {
            var dict = await this._documentStore.FindAsync<Dict>(command.Id);

            Assert.Build(dict)
                  .NotNull("dict not exist");

            var roots = await this._mediator.SendAsync<IEnumerable<DictItemDto>>(new DictCommand.Tree
            {
                ParentId = dict.Id
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

            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Tree.RootId;
            }

            var dicts = query.Select(s => new DictItemDto
            {
                Id = s.Id,
                Name = s.Name,
                ParentId = s.ParentId,
                Sort = s.Sort,
                CreateTime = s.CreateTime
            });

            if (command.Type.IsNullOrWhiteSpace())
            {
                var rootDicts = dicts.Where(s => s.ParentId == command.ParentId).ToArray();

                await rootDicts.ForEachAsync(async dict => await this.PermutationAsync(dict, dicts));

                return rootDicts;
            }
            else
            {
                var rootDicts = dicts.Where(s => s.ParentId == Constants.Tree.RootId && s.Name == command.Type).ToArray();

                await rootDicts.ForEachAsync(async dict => await this.PermutationAsync(dict, dicts));

                return rootDicts.FirstOrDefault()?.Items ?? Enumerable.Empty<DictItemDto>();
            }

        }

        public async Task<IEnumerable<DictDto>> HandleAsync(DictCommand.List command)
        {
            if (command.ParentId.IsNullOrWhiteSpace())
            {
                command.ParentId = Constants.Tree.RootId;
            }

            var query = this._storage.AsQueryable<DictDto>()
                                     .Where(s => s.ParentId == command.ParentId);

            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name == command.Name);
            }
            return query.ToArray();
        }

        public async Task<DictDto> HandleAsync(DictCommand.Find command)
        {
            if (string.IsNullOrWhiteSpace(command.Names))
            {
                return null;
            }

            var names = command.Names.Split(Constants.Tree.Separator).ToArray();

            var query = this._storage.AsQueryable<DictDto>();

            var dict = query.FirstOrDefault(s => s.ParentId == Constants.Tree.RootId && s.Name == names.First());

            if (dict != null && names.Length > 1)
            {
                foreach (var name in names.Skip(1))
                {
                    dict = query.FirstOrDefault(s => s.ParentId == dict.Id && s.Name == name);
                    
                    if (dict == null)
                    {
                        break;
                    }
                }
            }

            return dict;
        }
    }
}
