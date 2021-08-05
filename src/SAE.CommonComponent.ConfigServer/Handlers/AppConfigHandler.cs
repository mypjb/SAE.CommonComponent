using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Domains;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary;
using SAE.CommonLibrary.Extension;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Abstract.Model;
using SAE.CommonLibrary.Data;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Abstract.Builder;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.ConfigServer.Events;

namespace SAE.CommonComponent.ConfigServer.Handlers
{
    public class AppConfigHandler : AbstractHandler<AppConfig>,
                                    ICommandHandler<AppConfigCommand.ReferenceConfig>,
                                    ICommandHandler<Command.BatchDelete<AppConfig>>,
                                    ICommandHandler<AppConfigCommand.Query, IPagedList<AppConfigDto>>,
                                    ICommandHandler<AppConfigCommand.Query, IPagedList<ConfigDto>>,
                                    ICommandHandler<AppConfigCommand.Change>,
                                    ICommandHandler<Command.Find<AppConfigDto>, AppConfigDto>,
                                    ICommandHandler<AppConfigCommand.Publish>,
                                    ICommandHandler<AppConfigCommand.Preview, AppConfigDataPreviewDto>
    {
        private readonly IMediator _mediator;
        private readonly IDirector _director;
        private readonly IStorage _storage;
        public AppConfigHandler(
            IDocumentStore documentStore,
            IStorage storage,
            IMediator mediator,
            IDirector director) : base(documentStore)
        {
            this._mediator = mediator;
            this._director = director;
            this._storage = storage;
        }

        public async Task HandleAsync(AppConfigCommand.ReferenceConfig command)
        {
            if (!command.ConfigIds?.Any() ?? false)
            {
                return;
            }

            var appDto = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto>
            {
                Id = command.AppId
            });

            Assert.Build(appDto).IsNotNull();

            var appConfigs = this._storage.AsQueryable<Config>()
                                         .Where(s => command.ConfigIds.Contains(s.Id))
                                         .Select(s => new AppConfig(appDto.Id, s))
                                         .ToArray();


            appConfigs.ForEach(pc =>
            {
                if(this._storage.AsQueryable<AppConfigDto>()
                            .Where(s => s.AppId == pc.AppId &&
                                        s.EnvironmentId == pc.EnvironmentId &&
                                        s.Alias == pc.Alias)
                            .Count() > 0)
                {
                    pc.Alias += "_" + Guid.NewGuid().ToString("N");
                }
            });


            await this._documentStore.SaveAsync(appConfigs);
        }

        public async Task HandleAsync(Command.BatchDelete<AppConfig> command)
        {
            if (!command.Ids?.Any() ?? false)
            {
                return;
            }
            await this._documentStore.DeleteAsync<AppConfig>(command.Ids);
        }

        public async Task<IPagedList<AppConfigDto>> HandleAsync(AppConfigCommand.Query command)
        {
            var query = this._storage.AsQueryable<AppConfigDto>()
                                     .Where(s => s.AppId == command.AppId &&
                                            s.EnvironmentId == command.EnvironmentId);
            var paging = PagedList.Build(query, command);

            await this._director.Build(paging.AsEnumerable());

            return paging;
        }

        public async Task HandleAsync(AppConfigCommand.Change command)
        {
            var appConfig = await this._documentStore.FindAsync<AppConfig>(command.Id);

            appConfig.Change(command);

            Assert.Build(this._storage.AsQueryable<AppConfigDto>()
                             .Where(s => s.AppId == appConfig.AppId &&
                                    s.EnvironmentId == appConfig.EnvironmentId &&
                                    s.ConfigId != appConfig.ConfigId &&
                                    s.Alias == command.Alias)
                             .Count() == 0)
                  .True($"{command.Alias} is exist");

            await this._documentStore.SaveAsync(appConfig);
        }

        public async Task<AppConfigDto> HandleAsync(Command.Find<AppConfigDto> command)
        {
            return this._storage.AsQueryable<AppConfigDto>().First(s => s.Id == command.Id);
        }

        async Task<IPagedList<ConfigDto>> ICommandHandler<AppConfigCommand.Query, IPagedList<ConfigDto>>.HandleAsync(AppConfigCommand.Query command)
        {
            var appDto = await this._mediator.SendAsync<AppDto>(new Command.Find<AppDto>
            {
                Id = command.AppId
            });

            Assert.Build(appDto).IsNotNull();

            var query = this._storage.AsQueryable<ConfigDto>().Where(s => s.ClusterId == appDto.ClusterId &&
                                                                     s.EnvironmentId == command.EnvironmentId);

            var configIds = this._storage.AsQueryable<AppConfigDto>()
                               .Where(s => s.AppId == command.AppId &&
                                      s.EnvironmentId == command.EnvironmentId)
                               .Select(s => s.ConfigId)
                               .ToArray();

            query = query.Where(s => !configIds.Contains(s.Id));

            return PagedList.Build(query, command);
        }

        public async Task HandleAsync(AppConfigCommand.Publish command)
        {
            var tuple = await this.CombinationConfigAsync(command);

            var projectData = this._storage.AsQueryable<AppConfigData>()
                                           .FirstOrDefault(s => s.AppId == command.Id &&
                                                           s.EnvironmentId == command.EnvironmentId);

            if (projectData == null)
            {
                projectData = new AppConfigData(new AppConfigDataEvent.Create
                {
                    AppId = command.Id,
                    EnvironmentId = command.EnvironmentId,
                    Data = tuple.Item2.ToJsonString(),
                    PublicData = tuple.Item1.ToJsonString(),
                });
            }
            else
            {
                projectData = await this._documentStore.FindAsync<AppConfigData>(projectData.Id);
                projectData.Change(new AppConfigDataEvent.Publish
                {
                    Data = tuple.Item2.ToJsonString(),
                    PublicData = tuple.Item1.ToJsonString()
                });
            }

            await this._documentStore.SaveAsync(projectData);
        }

        public async Task<AppConfigDataPreviewDto> HandleAsync(AppConfigCommand.Preview command)
        {
            var tuple = await this.CombinationConfigAsync(command);
            return new AppConfigDataPreviewDto
            {
                Public = tuple.Item1,
                Private = tuple.Item2
            };
        }



        private async Task<Tuple<IDictionary<string, object>, IDictionary<string, object>>> CombinationConfigAsync(AppConfigCommand.Publish command)
        {
            var project = await this.FindAsync(command.Id);

            var projectConfigs = this._storage.AsQueryable<AppConfigDto>()
                                          .Where(s => s.AppId == command.Id &&
                                                 s.EnvironmentId == command.EnvironmentId)
                                          .ToArray();

            var configIds = projectConfigs.Select(p => p.ConfigId).ToArray();

            var configs = this._storage.AsQueryable<ConfigDto>()
                                       .Where(s => configIds.Contains(s.Id))
                                       .ToArray();

            var privateData = new Dictionary<string, object>();

            var publicData = new Dictionary<string, object>();

            foreach (var projectConfig in projectConfigs.Where(s => configs.Any(c => c.Id == s.ConfigId))
                                                                .ToArray())
            {
                var config = configs.FirstOrDefault(s => s.Id == projectConfig.ConfigId);

                var key = projectConfig.Alias;

                if (privateData.ContainsKey(key))
                {
                    key += "_";
                    privateData[key] = config?.Content.ToObject<object>();
                }
                else
                {
                    privateData[key] = config?.Content.ToObject<object>();
                }

                if (!projectConfig.Private)
                {
                    publicData[key] = privateData[key];
                }

            }

            return new Tuple<IDictionary<string, object>, IDictionary<string, object>>(publicData, privateData);
        }
    }
}
