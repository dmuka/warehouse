using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Resources.Dtos;
using Warehouse.Application.UseCases.Resources.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record UpdateResourceCommand(ResourceRequest Dto) : IRequest<Result<ResourceId>>;

public sealed class UpdateResourceCommandHandler(
    IResourceRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<UpdateResourceCommand, Result<ResourceId>>
{
    public async Task<Result<ResourceId>> Handle(
        UpdateResourceCommand request, 
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ResourceNameMustBeUnique(request.Dto.ResourceName, repository).IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<ResourceId>(specificationResult.Error);

        var resourceCreationResult = Resource.Create(request.Dto.ResourceName, request.Dto.IsActive, request.Dto.Id);
        if (resourceCreationResult.IsFailure) return Result.Failure<ResourceId>(resourceCreationResult.Error);
        
        repository.Update(resourceCreationResult.Value);
        cache.Remove(keyGenerator.ForMethod<Resource>(nameof(GetResourcesQueryHandler)));
        cache.Remove(keyGenerator.ForEntity<Resource>(resourceCreationResult.Value.Id));

        return Result.Success(resourceCreationResult.Value.Id);
    }
}