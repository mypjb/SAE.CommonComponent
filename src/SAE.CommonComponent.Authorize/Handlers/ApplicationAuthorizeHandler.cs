using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAE.CommonComponent.Application.Commands;
using SAE.CommonComponent.Application.Dtos;
using SAE.CommonComponent.Authorize.Commands;
using SAE.CommonComponent.Authorize.Dtos;
using SAE.CommonComponent.BasicData.Commands;
using SAE.CommonComponent.BasicData.Dtos;
using SAE.CommonLibrary.Abstract.Authorization.ABAC;
using SAE.CommonLibrary.Abstract.Mediator;
using SAE.CommonLibrary.AspNetCore.Authorization.ABAC;

namespace SAE.CommonComponent.Authorize.Handlers
{
    /// <summary>
    /// 应用授权处理器
    /// </summary>
    public class ApplicationAuthorizeHandler : ICommandHandler<ApplicationAuthorizeCommand.List, object>
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mediator"></param>
        public ApplicationAuthorizeHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        async Task<object> ICommandHandler<ApplicationAuthorizeCommand.List, object>.HandleAsync(ApplicationAuthorizeCommand.List command)
        {
            var appResourceDtos = await this._mediator.SendAsync<IEnumerable<AppResourceDto>>(new AppResourceCommand.List
            {
                ClusterId = command.ClusterId
            });

            var dict = await this._mediator.SendAsync<DictDto>(new DictCommand.Find
            {
                Names = Constants.Dict.AppResource
            });

            var strategyResourceDtos = await this._mediator.SendAsync<IEnumerable<StrategyResourceDto>>(new StrategyResourceCommand.List
            {
                ResourceType = dict.Id
            });

            var strategies = new List<StrategyDto>();

            var authDescriptors = new List<AuthDescriptor>();

            foreach (var appResource in appResourceDtos)
            {
                var strategyResources = strategyResourceDtos.Where(s => s.ResourceId == appResource.Id)
                                                            .ToArray();

                if (strategyResources.Any())
                {
                    var strategyIds = new List<string>();

                    foreach (var strategyResource in strategyResources)
                    {
                        if (strategyResource.Strategies != null && strategyResource.Strategies.Length > 0)
                        {
                            foreach (var strategy in strategyResource.Strategies)
                            {
                                if (!strategyIds.Contains(strategy.Id))
                                    strategyIds.Add(strategy.Id);

                                if (!strategies.Any(s => s.Id == strategy.Id))
                                    strategies.Add(strategy);
                            }
                        }
                    }

                    if (strategyIds.Any())
                    {
                        var authDescriptor = new AspNetCoreAuthDescriptor
                        {
                            Method = appResource.Method,
                            Name = appResource.Name,
                            Path = appResource.Path,
                            PolicyIds = strategyIds.Distinct().ToArray()
                        };

                        authDescriptors.Add(authDescriptor);
                    }
                }
            }

            return new
            {
                policies = strategies.ToArray(),
                descriptors = authDescriptors.ToArray()
            };
        }
    }
}