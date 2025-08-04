using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Units;

public class UnitId : TypedId
{
    protected UnitId() { }
    public UnitId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}