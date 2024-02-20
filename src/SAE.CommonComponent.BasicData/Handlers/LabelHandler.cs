using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// 
    /// </summary>
    public class LabelHandler : AbstractHandler<Label>,
                                ICommandHandler<LabelCommand.Create, string>,
                                ICommandHandler<Command.Delete<Label>>,
                                ICommandHandler<Command.Find<LabelDto>, LabelDto>,
                                ICommandHandler<LabelCommand.Find, LabelDto>,
                                ICommandHandler<LabelCommand.Query, IPagedList<LabelDto>>,
                                ICommandHandler<LabelCommand.List, IEnumerable<LabelDto>>,
                                ICommandHandler<LabelResourceCommand.Create>,
                                ICommandHandler<LabelResourceCommand.List, IEnumerable<LabelDto>>,
                                ICommandHandler<LabelResourceCommand.List, IEnumerable<string>>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentStore"></param>
        /// <param name="storage"></param>
        /// <param name="mediator"></param>
        /// <param name="director"></param>
        public LabelHandler(IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator,
            IDirector director) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._director = director;
        }



        private Task<bool> LabelIsExistAsync(Label label)
        {
            return Task.FromResult(this._storage.AsQueryable<LabelDto>()
                         .Count(s => s.Id != label.Id &&
                                     s.Name == label.Name &&
                                     s.Value == label.Value) > 0);
        }

        public async Task<string> HandleAsync(LabelCommand.Create command)
        {
            var dto = await this._mediator.SendAsync<LabelDto>(new LabelCommand.Find
            {
                Name = command.Name,
                Value = command.Value
            });

            if (dto != null)
            {
                return dto.Id;
            }
            else
            {
                var label = new Label(command);
                await this.AddAsync(label);
                return label.Id;
            }

        }

        public async Task<LabelDto> HandleAsync(Command.Find<LabelDto> command)
        {
            var first = this._storage.AsQueryable<LabelDto>()
                            .FirstOrDefault(s => s.Id == command.Id);
            if (first != null)
            {
                await this._director.Build<IEnumerable<LabelDto>>(new[] { first });
            }

            return first;
        }

        public async Task<IPagedList<LabelDto>> HandleAsync(LabelCommand.Query command)
        {
            var query = this._storage.AsQueryable<LabelDto>();

            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            var paging = PagedList.Build(query, command);

            await this._director.Build(paging.AsEnumerable());

            return paging;
        }

        public async Task HandleAsync(Command.Delete<Label> command)
        {
            var label = await this._documentStore.FindAsync<Label>(command.Id);

            Assert.Build(label).NotNull("label not exist");

            await this._documentStore.DeleteAsync(label);
        }

        public async Task<IEnumerable<LabelDto>> HandleAsync(LabelCommand.List command)
        {

            var query = this._storage.AsQueryable<LabelDto>();

            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name == command.Name);
            }

            return query.ToArray();
        }

        public Task<LabelDto> HandleAsync(LabelCommand.Find command)
        {
            var query = this._storage.AsQueryable<LabelDto>();
            var label = query.FirstOrDefault(s => s.Name == command.Name &&
                                    s.Value == command.Value);
            return Task.FromResult(label);
        }

        public async Task HandleAsync(LabelResourceCommand.Create command)
        {
            var labelDto = await this._mediator.SendAsync<LabelDto>(new LabelCommand.Create
            {
                Name = command.Name,
                Value = command.Value,
                Creator = command.Creator
            });

            var label = labelDto.To<Label>();

            var labelResource = new LabelResource(label, command);

            await this._storage.SaveAsync(labelResource);
        }

        public async Task<IEnumerable<LabelDto>> HandleAsync(LabelResourceCommand.List command)
        {
            if (command.ResourceId.IsNullOrWhiteSpace() || command.ResourceType.IsNullOrWhiteSpace())
            {
                return new LabelDto[0];
            }

            var labelIds = this._storage.AsQueryable<LabelResource>()
                                .Where(s => s.ResourceId == command.ResourceId && s.ResourceType == command.ResourceType)
                                .Select(s => s.LabelId)
                                .ToArray();

            var labels = this._storage.AsQueryable<LabelDto>()
                            .Where(s => labelIds.Contains(s.Id))
                            .ToArray();

            return labels;
        }

        async Task<IEnumerable<string>> ICommandHandler<LabelResourceCommand.List, IEnumerable<string>>.HandleAsync(LabelResourceCommand.List command)
        {
            if (command.ResourceId.IsNullOrWhiteSpace() || command.ResourceType.IsNullOrWhiteSpace())
            {
                return new string[0];
            }

            var labelIds = this._storage.AsQueryable<LabelResource>()
                                .Where(s => s.ResourceId == command.ResourceId && s.ResourceType == command.ResourceType)
                                .Select(s => s.LabelId)
                                .Distinct()
                                .ToArray();
            return labelIds;
        }
    }
}
