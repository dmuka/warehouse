using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Receipts.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Receipts;

public class Receipt : AggregateRoot
{
    [Key]
    public new ReceiptId Id { get; protected set; } = null!;
    public string Number { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public IReadOnlyCollection<ReceiptItem> Items => _items.AsReadOnly();

    private readonly List<ReceiptItem> _items = [];

    protected Receipt() { }

    private Receipt(string number, DateTime date, ReceiptId receiptId)
    {
        Number = number;
        Date = date;
        Id = receiptId;
    }

    public static Result<Receipt> Create(
        string number,
        DateTime date,
        Guid? receiptId = null)
    {
        var validationResults = ValidateReceiptDetails(number);
        if (validationResults.Length != 0)
            return Result<Receipt>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new Receipt(
            number,
            date,
            receiptId is null ? new ReceiptId(Guid.NewGuid()) : new ReceiptId(receiptId.Value));
    }

    public Result AddItem(ResourceId resourceId, UnitId unitId, decimal quantity)
    {
        var itemResult = ReceiptItem.Create(Id, resourceId, unitId, quantity);
        if (itemResult.IsFailure) return itemResult;

        if (_items.Any(i => i.ResourceId == resourceId && i.UnitId == unitId))
            return Result.Failure(ReceiptErrors.ReceiptItemAlreadyExist(resourceId, unitId));

        _items.Add(itemResult.Value);
        
        return Result.Success();
    }

    private static Result[] ValidateReceiptDetails(string number)
    {
        var results = new[]
        {
            new ReceiptNumberMustBeValid(number).IsSatisfied()
        };

        return results.Where(r => r.IsFailure).ToArray();
    }
}