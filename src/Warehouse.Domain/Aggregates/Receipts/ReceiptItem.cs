using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Receipts;

public class ReceiptItem : Entity
{
    public ReceiptId ReceiptId { get; private set; } = null!;
    public ResourceId ResourceId { get; private set; } = null!;
    public UnitId UnitId { get; private set; } = null!;
    public decimal Quantity { get; private set; }

    protected ReceiptItem() { }

    private ReceiptItem(
        ReceiptId receiptId,
        ResourceId resourceId,
        UnitId unitId,
        decimal quantity)
    {
        ReceiptId = receiptId;
        ResourceId = resourceId;
        UnitId = unitId;
        Quantity = quantity;
    }

    public static Result<ReceiptItem> Create(
        Guid resourceId,
        Guid unitId,
        decimal quantity)
    {
        var validationResults = ValidateItemDetails(resourceId, unitId, quantity);
        if (validationResults.Length != 0)
            return Result<ReceiptItem>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new ReceiptItem(
            new ReceiptId(Guid.Empty), // Temporary, will be set by ReceiptDocument
            new ResourceId(resourceId),
            new UnitId(unitId),
            quantity);
    }

    private static Result[] ValidateItemDetails(
        Guid resourceId,
        Guid unitId,
        decimal quantity)
    {
        var results = new List<Result>
        {
            new QuantityMustBePositive(quantity).IsSatisfied()
        };

        return results.Where(r => r.IsFailure).ToArray();
    }
}