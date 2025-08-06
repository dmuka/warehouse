using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record UnarchiveResourceCommand(Guid Id) : IRequest<Result>;

public sealed class UnarchiveResourceCommandHandler(
    IResourceRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UnarchiveResourceCommand, Result>
{
    public async Task<Result> Handle(UnarchiveResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(new ResourceId(request.Id), cancellationToken);
        if (resource is null) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));
        if (resource.IsActive) return Result.Failure<Resource>(ResourceErrors.ResourceAlreadyActive);
        
        resource.Activate();

        repository.Update(resource);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}