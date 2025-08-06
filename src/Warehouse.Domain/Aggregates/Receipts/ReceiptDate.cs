using Warehouse.Core;
using Warehouse.Core.Results;

namespace Warehouse.Domain.Aggregates.Receipts;

public class ReceiptDate : ValueObject
{
    protected ReceiptDate() { }
    public DateTime Value { get; private set; }

    private ReceiptDate(DateTime value) => Value = value;

    public static Result<ReceiptDate> Create(DateTime receiptDate)
    {
        return Result.Success(new ReceiptDate(receiptDate));
    }

    protected override IEnumerable<object> GetEqualityComponents() => [Value];
}