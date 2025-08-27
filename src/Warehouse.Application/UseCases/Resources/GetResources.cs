using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Resources.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record GetResourcesQuery : IRequest<Result<IList<ResourceResponse>>>;

public sealed class GetResourcesQueryHandler(
    IResourceRepository resourceRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<GetResourcesQueryHandler> logger) : IRequestHandler<GetResourcesQuery, Result<IList<ResourceResponse>>>
{
    public async Task<Result<IList<ResourceResponse>>> Handle(
        GetResourcesQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = keyGenerator.ForMethod<Resource>(nameof(GetResourcesQueryHandler));
        var response = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var resources = await resourceRepository.GetListAsync(cancellationToken);
            var response = resources
                .Select(resource => new ResourceResponse(resource.Id.Value, resource.ResourceName.Value, resource.IsActive))
                .ToList();

            return response;
        });

        return Result.Success<IList<ResourceResponse>>(response);
    }
}