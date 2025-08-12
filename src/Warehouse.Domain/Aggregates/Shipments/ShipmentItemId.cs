using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Shipments;

public class ShipmentItemId : TypedId
{
    protected ShipmentItemId() { }
    public ShipmentItemId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}