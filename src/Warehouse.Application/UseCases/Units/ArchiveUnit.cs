using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Units;
using Unit = MediatR.Unit;

namespace Warehouse.Application.UseCases.Units;

public record ArchiveUnitCommand(Guid Id) : IRequest<Result>;

public sealed class ArchiveUnitCommandHandler(
    IUnitRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<ArchiveUnitCommand, Result>
{
    public async Task<Result> Handle(ArchiveUnitCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(new UnitId(request.Id), cancellationToken);
        if (resource is null) return Result.Failure<Unit>(UnitErrors.NotFound(request.Id));
        if (resource.IsActive == false) return Result.Failure<Unit>(UnitErrors.UnitAlreadyArchived);
        
        resource.Deactivate();

        repository.Update(resource);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}