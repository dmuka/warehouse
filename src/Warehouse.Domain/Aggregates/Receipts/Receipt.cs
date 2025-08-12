using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Receipts.DomainEvents;
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

    private IList<ReceiptItem> _items = [];

    protected Receipt() { }

    private Receipt(
        string number, 
        DateTime date, 
        IList<ReceiptItem> items, 
        ReceiptId receiptId)
    {
        Number = number;
        Date = date;
        _items = items;
        Id = receiptId;
    }

    public static Result<Receipt> Create(
        string number,
        DateTime date,
        IList<ReceiptItem> items,
        Guid? receiptId = null)
    {
        var validationResults = ValidateReceiptDetails(number);
        if (validationResults.Length != 0)
            return Result<Receipt>.ValidationFailure(ValidationError.FromResults(validationResults));
        
        var receipt = new Receipt(
            number,
            date,
            items,
            receiptId is null ? new ReceiptId(Guid.NewGuid()) : new ReceiptId(receiptId.Value));
        
        receipt.AddDomainEvent(new ReceiptCreatedDomainEvent(
            receipt.Id,
            items.Select(i => ReceiptItem.Create(
                receipt.Id,
                i.ResourceId.Value,
                i.UnitId.Value,
                i.Quantity,
                i.Id).Value).ToList()));

        return receipt;
    }
    
    public void Remove()
    {
        AddDomainEvent(new ReceiptRemovedDomainEvent(
            Id,
            Items.ToList()));
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
        Result[] results = [];

        return results.Where(r => r.IsFailure).ToArray();
    }
}