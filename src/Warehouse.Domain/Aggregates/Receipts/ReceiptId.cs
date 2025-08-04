using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Receipts;

public class ReceiptId : TypedId
{
    protected ReceiptId() { }
    public ReceiptId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}