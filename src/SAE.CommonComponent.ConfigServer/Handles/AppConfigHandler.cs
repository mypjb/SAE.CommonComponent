using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Dtos;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Handles
{
    public class AppConfigHandler : ICommandHandler<AppCommand.Config, AppConfigDto>
    {
        private readonly IStorage _storage;

        public AppConfigHandler(IStorage storage)
        {
            this._storage = storage;
        }
        public async Task<AppConfigDto> Handle(AppCommand.Config command)
        {
            var app = new AppConfigDto();

            var projectDto = this._storage.AsQueryable<ProjectDto>().FirstOrDefault(s => s.Id == command.Id);

            app.Version=projectDto.Version;

            if (projectDto.Version != command.Version)
            {
                
                var projectConfigs = this._storage.AsQueryable<ProjectConfigDto>()
                                                              .Where(s => s.ProjectId == command.Id);

                var configs = this._storage.AsQueryable<ConfigDto>()
                                           .Where(s => projectConfigs.Any(pc => pc.ConfigId == s.Id))
                                           .ToArray();

                foreach (var projectConfig in projectConfigs)
                {
                    app.Add(projectConfig, configs.FirstOrDefault(s => s.Id == projectConfig.ConfigId));
                }
            }

            return app;
        }
    }
}
