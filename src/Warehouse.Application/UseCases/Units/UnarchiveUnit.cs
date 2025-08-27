using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record UnarchiveUnitCommand(Guid Id) : IRequest<Result>;

public sealed class UnarchiveUnitCommandHandler(
    IUnitRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<UnarchiveUnitCommand, Result>
{
    public async Task<Result> Handle(UnarchiveUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Id), cancellationToken);
        if (unit is null) return Result.Failure<Unit>(UnitErrors.NotFound(request.Id));
        if (unit.IsActive) return Result.Failure<Unit>(UnitErrors.UnitAlreadyActive);
        
        unit.Activate();

        repository.Update(unit);
        cache.Remove(keyGenerator.ForMethod<Unit>(nameof(GetUnitsQueryHandler)));
        cache.RemoveAllForEntity<Unit>(unit.Id);
        
        return Result.Success();
    }
}