using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Shipments;

public class ShipmentId : TypedId
{
    protected ShipmentId() { }
    public ShipmentId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}