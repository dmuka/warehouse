using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record ArchiveResourceCommand(Guid Id) : IRequest<Result>;

public sealed class ArchiveResourceCommandHandler(
    IResourceRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<ArchiveResourceCommand, Result>
{
    public async Task<Result> Handle(ArchiveResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (resource is null) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));
        if (resource.IsActive == false) return Result.Failure<Resource>(ResourceErrors.ResourceAlreadyArchived);
        
        resource.Deactivate();

        repository.Update(resource);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}