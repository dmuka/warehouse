using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Resources.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record GetResourceByIdQuery(Guid Id) : IRequest<Result<ResourceResponse>>;

public sealed class GetResourceByIdQueryHandler(
    IResourceRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetResourceByIdQuery, Result<ResourceResponse>>
{
    public async Task<Result<ResourceResponse>> Handle(GetResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ResourceId(request.Id);
        var cacheKey = keyGenerator.ForEntity<Resource>(id);
        var resource = await cache.GetOrCreateAsync(cacheKey, async () => await repository.GetByIdAsync(id, cancellationToken));
        if (resource is null) return Result.Failure<ResourceResponse>(ResourceErrors.NotFound(request.Id));
        
        var response = new ResourceResponse(resource.Id.Value, resource.ResourceName.Value, resource.IsActive);

        return response;
    }
}