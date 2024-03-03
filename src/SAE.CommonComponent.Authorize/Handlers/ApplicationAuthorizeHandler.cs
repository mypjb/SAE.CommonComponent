using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using SAE.CommonLibrary.Caching;
using SAE.CommonLibrary.Extension;

namespace SAE.CommonComponent.Authorize.Handlers
{
    /// <summary>
    /// 应用授权处理器
    /// </summary>
    public class ApplicationAuthorizeHandler : ICommandHandler<ApplicationAuthorizeCommand.Find, ApplicationAuthorizeDto>
    {
        private readonly IMediator _mediator;
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="distributedCache"></param>
        public ApplicationAuthorizeHandler(IMediator mediator, IDistributedCache distributedCache)
        {
            this._mediator = mediator;
            this._distributedCache = distributedCache;
        }

        async Task<ApplicationAuthorizeDto> ICommandHandler<ApplicationAuthorizeCommand.Find, ApplicationAuthorizeDto>.HandleAsync(ApplicationAuthorizeCommand.Find command)
        {

            var applicationAuthorizeDto = await this._distributedCache.GetOrAddAsync(command.ToString(), () => this.FindCoreAsync(command));

            return applicationAuthorizeDto;
        }

        private async Task<ApplicationAuthorizeDto> FindCoreAsync(ApplicationAuthorizeCommand.Find command)
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

            var authDescriptors = new List<AspNetCoreAuthDescriptor>();

            var sb = new StringBuilder();

            foreach (var appResource in appResourceDtos)
            {
                var strategyResources = strategyResourceDtos.Where(s => s.ResourceId == appResource.Id)
                                                            .ToArray();

                if (strategyResources.Any())
                {
                    var strategyIds = new List<string>();

                    foreach (var strategyResourceGroup in strategyResources.GroupBy(s => s.ResourceId).ToArray())
                    {
                        foreach (var strategyResource in strategyResourceGroup)
                        {
                            if (strategyResource.Strategy.Status != Status.Enable)
                                continue;

                            if (!strategyIds.Contains(strategyResource.StrategyId))
                                strategyIds.Add(strategyResource.StrategyId);

                            if (!strategies.Any(s => s.Id == strategyResource.StrategyId))
                                strategies.Add(strategyResource.Strategy);
                        }
                    }

                    if (strategyIds.Any())
                    {
                        var authDescriptor = new AspNetCoreAuthDescriptor
                        {
                            Method = appResource.Method,
                            Name = appResource.Name,
                            Path = appResource.Path,
                            PolicyIds = strategyIds.OrderBy(s => s).Distinct().ToArray()
                        };

                        authDescriptors.Add(authDescriptor);
                    }
                }
            }

            IEnumerable<AuthorizationPolicy> policies;

            if (strategies.Any())
            {
                policies = strategies.Select(s => new AuthorizationPolicy
                {
                    Id = s.Id,
                    Name = s.Name,
                    Rule = s.Expression
                }).OrderBy(s => s.Id).ToArray();

                sb.Append(nameof(policies)).Append(":");

                foreach (var policy in policies)
                {
                    sb.Append(policy.Id);
                }
            }
            else
            {
                policies = Enumerable.Empty<AuthorizationPolicy>();
            }

            IEnumerable<AspNetCoreAuthDescriptor> descriptors;

            if (authDescriptors.Any())
            {
                descriptors = authDescriptors.OrderBy(s => s.Key).ToArray();

                sb.Append(nameof(descriptors)).Append(":");

                foreach (var descriptor in descriptors)
                {
                    sb.Append(descriptor.Key);

                    if (descriptor.PolicyIds.Any())
                    {
                        descriptor.PolicyIds.ForEach(s => sb.Append(s));
                    }
                }
            }
            else
            {
                descriptors = Enumerable.Empty<AspNetCoreAuthDescriptor>();
            }

            return new ApplicationAuthorizeDto
            {
                Data = new
                {
                    Policies = policies,
                    Descriptors = descriptors
                },
                Version = sb.Length > 0 ? sb.ToString().ToMd5().ToLower() : "0"
            };
        }
    }
}