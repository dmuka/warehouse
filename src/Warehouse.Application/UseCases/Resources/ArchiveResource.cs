using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record ArchiveResourceCommand(Guid Id) : IRequest<Result>;

public sealed class ArchiveResourceCommandHandler(
    IResourceRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<ArchiveResourceCommand, Result>
{
    public async Task<Result> Handle(ArchiveResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(new ResourceId(request.Id), cancellationToken);
        if (resource is null) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));
        if (!resource.IsActive) return Result.Failure<Resource>(ResourceErrors.ResourceAlreadyArchived);
        
        resource.Deactivate();

        repository.Update(resource);
        cache.Remove(keyGenerator.ForMethod<Client>(nameof(GetResourcesQueryHandler)));
        cache.Remove(keyGenerator.ForEntity<Resource>(resource.Id));
        
        return Result.Success();
    }
}