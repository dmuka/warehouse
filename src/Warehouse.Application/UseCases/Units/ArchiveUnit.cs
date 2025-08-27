using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record ArchiveUnitCommand(Guid Id) : IRequest<Result>;

public sealed class ArchiveUnitCommandHandler(
    IUnitRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<ArchiveUnitCommand, Result>
{
    public async Task<Result> Handle(ArchiveUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Id), cancellationToken);
        if (unit is null) return Result.Failure<Unit>(UnitErrors.NotFound(request.Id));
        if (!unit.IsActive) return Result.Failure<Unit>(UnitErrors.UnitAlreadyArchived);
        
        unit.Deactivate();

        repository.Update(unit);
        cache.Remove(keyGenerator.ForMethod<Unit>(nameof(GetUnitsQueryHandler)));
        cache.RemoveAllForEntity<Unit>(unit.Id);
        
        return Result.Success();
    }
}