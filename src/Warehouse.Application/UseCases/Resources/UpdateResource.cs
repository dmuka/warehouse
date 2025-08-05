using MediatR;
using Warehouse.Application.UseCases.Resources.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Resources;

public record UpdateResourceCommand(ResourceDto Dto) : IRequest<Result<ResourceId>>;

public sealed class UpdateResourceCommandHandler(
    IResourceRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateResourceCommand, Result<ResourceId>>
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
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(resourceCreationResult.Value.Id);
    }
}