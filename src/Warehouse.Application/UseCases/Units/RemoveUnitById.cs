using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record RemoveUnitByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveUnitByIdQueryHandler(
    IUnitRepository repository,
    IReceiptRepository receiptRepository,
    IShipmentRepository shipmentRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<RemoveUnitByIdQuery, Result>
{
    public async Task<Result> Handle(RemoveUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new UnitId(request.Id);
        var unit = await repository.GetByIdAsync(id, cancellationToken);
        if (unit is null) return Result.Failure<Unit>(UnitErrors.NotFound(request.Id));

        var unitInUse = shipmentRepository.GetQueryable().Any(shipment => shipment.Items.Any(item => item.UnitId == unit.Id));
        if (unitInUse) return Result<ResourceId>.ValidationFailure(UnitErrors.UnitIsInUse(request.Id));
        unitInUse = receiptRepository.GetQueryable().Any(receipt => receipt.Items.Any(item => item.UnitId == unit.Id));
        if (unitInUse) return Result<ResourceId>.ValidationFailure(UnitErrors.UnitIsInUse(request.Id));

        repository.Delete(unit);
        await repository.SaveChangesAsync(cancellationToken);
        cache.Remove(keyGenerator.ForMethod<Unit>(nameof(GetUnitsQueryHandler)));
        cache.RemoveAllForEntity<Unit>(unit.Id);

        return Result.Success();
    }
}