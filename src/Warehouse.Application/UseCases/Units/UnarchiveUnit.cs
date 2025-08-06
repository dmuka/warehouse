using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record UnarchiveUnitCommand(Guid Id) : IRequest<Result>;

public sealed class UnarchiveUnitCommandHandler(
    IUnitRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UnarchiveUnitCommand, Result>
{
    public async Task<Result> Handle(UnarchiveUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Id), cancellationToken);
        if (unit is null) return Result.Failure<Unit>(UnitErrors.NotFound(request.Id));
        if (unit.IsActive) return Result.Failure<Unit>(UnitErrors.UnitAlreadyActive);
        
        unit.Activate();

        repository.Update(unit);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}