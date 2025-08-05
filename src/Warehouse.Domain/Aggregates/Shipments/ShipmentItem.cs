using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Shipments;

public class ShipmentItem : Entity
{
    public ShipmentId ShipmentId { get; private set; } = null!;
    public ResourceId ResourceId { get; private set; } = null!;
    public UnitId UnitId { get; private set; } = null!;
    public decimal Quantity { get; private set; }

    protected ShipmentItem() { }

    private ShipmentItem(
        ShipmentId shipmentId,
        ResourceId resourceId,
        UnitId unitId,
        decimal quantity)
    {
        ShipmentId = shipmentId;
        ResourceId = resourceId;
        UnitId = unitId;
        Quantity = quantity;
    }

    public static Result<ShipmentItem> Create(
        ShipmentId shipmentId,
        ResourceId resourceId,
        UnitId unitId,
        decimal quantity)
    {
        var validationResults = ValidateItemDetails(quantity);
        if (validationResults.Length != 0)
            return Result<ShipmentItem>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new ShipmentItem(
            shipmentId,
            resourceId,
            unitId,
            quantity);
    }

    private static Result[] ValidateItemDetails(decimal quantity)
    {
        var results = new List<Result>
        {
            new QuantityMustBePositive(quantity).IsSatisfied()
        };

        return results.Where(r => r.IsFailure).ToArray();
    }
}