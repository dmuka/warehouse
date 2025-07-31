using MediatR;
using Warehouse.Application.UseCases.Resources.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Resources;

public record CreateResourceCommand(ResourceDto Dto) : IRequest<Result<ResourceId>>;

public sealed class CreateResourceCommandHandler(
    IResourceRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateResourceCommand, Result<ResourceId>>
{
    public async Task<Result<ResourceId>> Handle(
        CreateResourceCommand request, 
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ResourceNameMustBeUnique(request.Dto.ResourceName, repository).IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<ResourceId>(specificationResult.Error);

        var resourceCreationResult = Resource.Create(request.Dto.ResourceName, request.Dto.IsActive, request.Dto.Id);
        if (resourceCreationResult.IsFailure) return Result.Failure<ResourceId>(resourceCreationResult.Error);
        
        repository.Add(resourceCreationResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(resourceCreationResult.Value.Id);
    }
}