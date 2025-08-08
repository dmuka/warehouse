using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Receipts;

public class ReceiptItem : Entity
{
    [Key]
    public new ReceiptItemId Id { get; protected set; } = null!;
    public ReceiptId ReceiptId { get; private set; } = null!;
    public ResourceId ResourceId { get; private set; } = null!;
    public UnitId UnitId { get; private set; } = null!;
    public decimal Quantity { get; private set; }

    protected ReceiptItem() { }

    private ReceiptItem(
        ReceiptId receiptId,
        ResourceId resourceId,
        UnitId unitId,
        decimal quantity,
        ReceiptItemId receiptItemId)
    {
        Id = receiptItemId; 
        ReceiptId = receiptId;
        ResourceId = resourceId;
        UnitId = unitId;
        Quantity = quantity;
    }

    public static Result<ReceiptItem> Create(
        Guid receiptId,
        Guid resourceId,
        Guid unitId,
        decimal quantity,
        Guid? receiptItemId = null)
    {
        var validationResults = ValidateItemDetails(quantity);
        if (validationResults.Length != 0)
            return Result<ReceiptItem>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new ReceiptItem(
            new ReceiptId(receiptId),
            new ResourceId(resourceId),
            new UnitId(unitId),
            quantity,
            receiptItemId is null ? new ReceiptItemId(Guid.CreateVersion7()) : new ReceiptItemId(receiptItemId.Value));
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