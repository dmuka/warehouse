using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Receipts;

public class ReceiptItemId : TypedId
{
    protected ReceiptItemId() { }
    public ReceiptItemId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}