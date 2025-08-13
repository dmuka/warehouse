using MediatR;
using Warehouse.Application.UseCases.Resources.Dtos;
using Warehouse.Application.UseCases.Resources.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record CreateResourceCommand(ResourceRequest Request) : IRequest<Result<ResourceId>>;

public sealed class CreateResourceCommandHandler(
    IResourceRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateResourceCommand, Result<ResourceId>>
{
    public async Task<Result<ResourceId>> Handle(
        CreateResourceCommand request, 
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ResourceNameMustBeUnique(request.Request.ResourceName, repository)
            .IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<ResourceId>(specificationResult.Error);

        var resourceCreationResult = Resource.Create(request.Request.ResourceName, request.Request.IsActive);
        if (resourceCreationResult.IsFailure) return Result.Failure<ResourceId>(resourceCreationResult.Error);
        
        repository.Add(resourceCreationResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(resourceCreationResult.Value.Id);
    }
}