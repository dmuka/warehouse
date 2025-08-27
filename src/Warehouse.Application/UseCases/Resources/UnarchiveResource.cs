using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record UnarchiveResourceCommand(Guid Id) : IRequest<Result>;

public sealed class UnarchiveResourceCommandHandler(
    IResourceRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<UnarchiveResourceCommand, Result>
{
    public async Task<Result> Handle(UnarchiveResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(new ResourceId(request.Id), cancellationToken);
        if (resource is null) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));
        if (resource.IsActive) return Result.Failure<Resource>(ResourceErrors.ResourceAlreadyActive);
        
        resource.Activate();

        repository.Update(resource);
        cache.Remove(keyGenerator.ForMethod<Resource>(nameof(GetResourcesQueryHandler)));
        cache.Remove(keyGenerator.ForEntity<Resource>(resource.Id));
        
        return Result.Success();
    }
}