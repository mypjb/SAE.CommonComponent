using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Domains;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.AspNetCore.Authorization;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Logging;
using SAE.CommonLibrary.MessageQueue;

namespace SAE.CommonComponent.Application.Abstract.Handlers
{

    public class AppResourceHandler : AbstractHandler<AppResource>,
                                      ICommandHandler<AppResourceCommand.Create, string>,
                                      ICommandHandler<AppResourceCommand.Change>,
                                      ICommandHandler<AppResourceCommand.SetIndex>,
                                      ICommandHandler<Command.Delete<AppResource>>,
                                      ICommandHandler<AppResourceCommand.Query, IPagedList<AppResourceDto>>,
                                      ICommandHandler<Command.Find<AppResourceDto>, AppResourceDto>,
                                      ICommandHandler<AppResourceCommand.List, IEnumerable<AppResourceDto>>,
                                      ICommandHandler<AppResourceCommand.BitmapEndpoints, BitmapEndpointListDto>
    {
        private readonly IStorage _storage;
        private readonly IMediator _mediator;
        private readonly ILogging _logging;
        private readonly IMessageQueue _messageQueue;

        public AppResourceHandler(IDocumentStore documentStore,
                                  IStorage storage,
                                  IMediator mediator,
                                  ILogging<AppHandler> logging,
                                  IMessageQueue messageQueue) : base(documentStore)
        {
            this._storage = storage;
            this._mediator = mediator;
            this._logging = logging;
            this._messageQueue = messageQueue;
        }


        public async Task HandleAsync(AppResourceCommand.Change command)
        {
            var appResource = await this._documentStore.FindAsync<AppResource>(command.Id);
            appResource.Change(command);
            await this._documentStore.SaveAsync(appResource);
        }

        public async Task<string> HandleAsync(AppResourceCommand.Create command)
        {
            var appResource = new AppResource(command);
            await this.AddAsync(appResource);
            await this._messageQueue.PublishAsync(command);
            return appResource.Id;
        }


        public async Task<AppResourceDto> HandleAsync(Command.Find<AppResourceDto> command)
        {
            var dto = this._storage.AsQueryable<AppResourceDto>()
                          .FirstOrDefault(s => s.Id == command.Id);
            return dto;
        }

        public Task<IPagedList<AppResourceDto>> HandleAsync(AppResourceCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppResourceDto>();
            if (!command.Name.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.Name.Contains(command.Name));
            }

            return Task.FromResult(PagedList.Build(query, command));
        }

        public async Task HandleAsync(Command.Delete<AppResource> command)
        {
            await this.DeleteAsync(command.Id);
        }

        public Task<IEnumerable<AppResourceDto>> HandleAsync(AppResourceCommand.List command)
        {
            return Task.FromResult(this._storage.AsQueryable<AppResourceDto>()
                 .Where(s => s.AppId == command.AppId)
                 .ToArray()
                 .AsEnumerable());
        }

        public async Task HandleAsync(AppResourceCommand.SetIndex command)
        {
            var resource = await this.FindAsync(command.Id);
            Assert.Build(resource)
                  .NotNull("资源不存在，或被删除！");
            resource.SetIndex(command);
            await this._documentStore.SaveAsync(resource);
        }

        public async Task<BitmapEndpointListDto> HandleAsync(AppResourceCommand.BitmapEndpoints command)
        {
            var query = this._storage.AsQueryable<AppResourceDto>();

            Assert.Build(command.AppId.IsNullOrWhiteSpace() && command.ClusterId.IsNullOrWhiteSpace())
                  .False("请提供集群或系统标识");

            if (!command.ClusterId.IsNullOrWhiteSpace())
            {
                query = from ar in query
                        join app in this._storage.AsQueryable<AppDto>()
                        on ar.AppId equals app.Id
                        where app.ClusterId == command.ClusterId
                        select ar;
            }
            else if (!command.AppId.IsNullOrWhiteSpace())
            {
                query = query.Where(s => s.AppId == command.AppId);
            }

            var appResources = query.OrderBy(s => s.Index)
                             .ToArray();

            var sb = new StringBuilder();

            appResources.ForEach(ap =>
            {
                if (sb.Length > 0)
                {
                    sb.Append(Constants.DefaultSeparator);
                }
                sb.Append(ap.Id)
                  .Append(Constants.DefaultSeparator);
            });

            var bitmapEndpointList = new BitmapEndpointListDto
            {
                Version = sb.ToString().ToMd5()
            };

            var dict = appResources.GroupBy(s => s.AppId)
                            .ToDictionary(s => s.Key,
                                s => new Dictionary<string, object>
                                {
                                    {
                                        $"{ConfigurationEndpointOptions.Option}{SAE.CommonLibrary.Configuration.Constants.ConfigSeparator}{nameof(ConfigurationEndpointOptions.BitmapEndpoints)}",
                                        s.Select(ap=> new BitmapEndpoint
                                        {
                                            Index=ap.Index,
                                            Name=ap.Name,
                                            Path=ap.Path,
                                            Method=ap.Method
                                        })
                                    }
                                });

            bitmapEndpointList.Data = dict;

            return bitmapEndpointList;
        }
    }
}